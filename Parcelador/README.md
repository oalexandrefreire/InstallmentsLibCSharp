# Example
using InstallmentLib;
using InstallmentLib.Enuns;
using System.Text.Json;

var parcelas = Installment.Generate(
        value: 999,
        firstDueDate: DateTime.Now,
        quantityInstallments: 7,
        valueType: ValueTypeEnum.Amount,
        dueDateOnlyOnBussinesDay: true,
        apportionmentType: ApportionmentTypeEnum.ProratedBetweenInstallmentsDescending        
    );

var parcelas2 = Installment.Generate(
        value: 25,
        firstDueDate: DateTime.Now,
        quantityInstallments: 7,
        valueType: ValueTypeEnum.InstallmentValue
    );

foreach (var item in parcelas)
{
    Console.WriteLine(JsonSerializer.Serialize(item));
}
Console.WriteLine("---------------------------------------------------");

foreach (var itemb in parcelas2)
{
    Console.WriteLine(JsonSerializer.Serialize(itemb));
}
Console.WriteLine("---------------------------------------------------");