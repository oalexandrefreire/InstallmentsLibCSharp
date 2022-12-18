using InstallmentLib.Enuns;
using InstallmentLib.Models;
using NextBusinessDay;
using System;
using System.Collections.Generic;
using System.Linq;

namespace InstallmentLib
{
    public static class Installment
    {

        public static List<InstallmentModel> Generate(
            decimal value, 
            DateTime firstDueDate, 
            int quantityInstallments = 1,
            ValueTypeEnum valueType = ValueTypeEnum.Amount,
            bool dueDateOnlyOnBussinesDay = false,
            ApportionmentTypeEnum apportionmentType = ApportionmentTypeEnum.None
            )
        {
            if (value <= 0)
                throw new InvalidOperationException("Value must be greater than zero.");

            if (quantityInstallments <= 0)
                throw new InvalidOperationException("Quantity must be greater than zero.");

            var response = new SortedDictionary<int, decimal>();
            var installments = ExactInstallment(valueType, value, quantityInstallments, firstDueDate, dueDateOnlyOnBussinesDay);

            if (valueType == ValueTypeEnum.InstallmentValue || apportionmentType == ApportionmentTypeEnum.None)
                return installments;


            switch (apportionmentType)
            {
                case ApportionmentTypeEnum.FirstInstallment:
                    response = Installment.FirstInstallment(value, quantityInstallments);
                    break;
                case ApportionmentTypeEnum.LastInstallment:
                    response = Installment.LastInstallment(value, quantityInstallments);
                    break;
                case ApportionmentTypeEnum.ProratedBetweenInstallmentsAscending:
                    response = Installment.ProratedBetweenInstallmentsAscending(value, quantityInstallments);
                    break;
                case ApportionmentTypeEnum.ProratedBetweenInstallmentsDescending:
                    response = Installment.ProratedBetweenInstallmentsDescending(value, quantityInstallments);
                    break;
            }
            
            var installmentsNew = new List<InstallmentModel>();

            for (int i = 0; i < quantityInstallments; i++)
            {
                var newValue = response
                    .Where(x => x.Key == installments[i].Number)
                    .FirstOrDefault().Value;
                var installment = new InstallmentModel(Guid.NewGuid(), i + 1, newValue, installments[i].DueDate
                    );


                installmentsNew.Add(installment);
            }

            return installmentsNew;
        }


        private static List<InstallmentModel> ExactInstallment(ValueTypeEnum valueType, decimal value, int quantityInstallments, DateTime firstDueDate, bool dueDateOnlyOnBussinesDay)
        {
            var installments = new List<Models.InstallmentModel>();

            for (int i = 0; i < quantityInstallments; i++)
            {
                decimal installmentValue = value;
               
                if (valueType == ValueTypeEnum.Amount)
                    installmentValue = (decimal)(value / quantityInstallments);

                var dueDateCustom = firstDueDate.AddMonths(i);
                if (dueDateOnlyOnBussinesDay) 
                    dueDateCustom = BusinessDayLib.GetNextBusinessDay(firstDueDate.AddMonths(i));

                var installment = new InstallmentModel(Guid.NewGuid(), i + 1, 
                    installmentValue, dueDateCustom.ToString("yyyy-MM-dd"));

                installments.Add(installment);
            }
            return installments;
        }

        private static SortedDictionary<int, decimal> FirstInstallment(
            decimal valor, int quantidadeParcelas
            )
        {
            return Simples(valor, quantidadeParcelas,
                (parcelas, resto) =>
                {
                    if (resto > 0)
                    {
                        parcelas[1] = parcelas[1] + resto;
                    }
                    return parcelas;
                }
            );

        }

        private static SortedDictionary<int, decimal> LastInstallment(decimal valor, int quantidadeParcelas)
        {
            return Simples(valor, quantidadeParcelas,
                (parcelas, resto) =>
                {

                    if (resto > 0)
                    {
                        var idParcela = parcelas.Count;
                        parcelas[idParcela] = parcelas[idParcela] + resto;
                    }

                    return parcelas;
                });
        }

        private static SortedDictionary<int, decimal> ProratedBetweenInstallmentsAscending(decimal valor, int quantidadeParcelas)
        {
            return Simples(valor, quantidadeParcelas,
                (parcelas, resto) =>
                {

                    if (resto > 0)
                    {
                        var ultimaParcela = parcelas.Count;
                        var idParcela = 0;
                        for (int i = (int)Math.Truncate(resto * 100); i > 0; i--)
                        {
                            idParcela++;

                            parcelas[idParcela] = parcelas[idParcela] + 0.01M;
                            if (idParcela == ultimaParcela)
                                idParcela = 1;
                        }
                    }

                    return parcelas;
                });
        }

        private static SortedDictionary<int, decimal> ProratedBetweenInstallmentsDescending(decimal valor, int quantidadeParcelas)
        {
            return Simples(valor, quantidadeParcelas, (parcelas, resto) =>
            {
                if (resto > 0)
                {
                    var ultimaParcela = parcelas.Count;
                    var idParcela = ultimaParcela;
                    for (int i = (int)Math.Truncate(resto * 100); i > 0; i--)
                    {
                        parcelas[idParcela] = parcelas[idParcela] + 0.01M;

                        idParcela--;

                        if (idParcela == 0)
                            idParcela = ultimaParcela;
                    }
                }
                return parcelas;
            });
        }

        private static SortedDictionary<int, decimal> Simples(decimal valor, int quantidadeParcelas, Func<SortedDictionary<int, decimal>, decimal, SortedDictionary<int, decimal>> rateio)
        {
            var valorModulo = Math.Abs(valor);

            var parcelas = new SortedDictionary<int, decimal>();
            decimal valorTotal = 0;
            if (quantidadeParcelas <= 1)
            {
                parcelas.Add(1, valorModulo);
                valorTotal = valorModulo;
            }
            else
            {
                decimal valorParcela = Math.Truncate((valorModulo / (decimal)quantidadeParcelas) * 100) / 100;

                for (var i = 1; i <= quantidadeParcelas; i++)
                {
                    parcelas.Add(i, valorParcela);
                    valorTotal += valorParcela;
                }
            }

            decimal resto = valorModulo - valorTotal;

            parcelas = rateio(parcelas, resto);

            if (valor < 0)
            {
                parcelas.Keys.ToList().ForEach(key =>
                {
                    parcelas[key] = parcelas[key] * -1;
                });
            }

            return parcelas;
        }

    }

}
