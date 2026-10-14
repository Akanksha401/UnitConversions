using UnitConversions.Models;
namespace UnitConversions.Services
{
    public interface IUnitConverterService
    {
        ConversionResponse Convert(ConversionRequest request);
        IEnumerable<string> GetSupportedCategories();
        IEnumerable<string> GetSupportedUnits(string category);
    }
}
