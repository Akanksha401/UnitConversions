using UnitConversions.Services;
using UnitConversions.Models;
namespace UnitConversions.Services
{
    public class UnitConverterService : IUnitConverterService
    {
        private readonly Dictionary<string, Dictionary<string, double>> _linearUnits = new(StringComparer.OrdinalIgnoreCase)
        {
            {
                "length", new(StringComparer.OrdinalIgnoreCase)
                {
                    { "meters", 1.0 },
                    { "kilometers", 1000.0 },
                    { "centimeters", 0.01 },
                    { "millimeters", 0.001 },
                    { "miles", 1609.344 },
                    { "feet", 0.3048 },
                    { "inches", 0.0254 }
                }
            },
            {
                "weight", new(StringComparer.OrdinalIgnoreCase) // Handles mass/weight
                {
                    { "kilograms", 1.0 },
                    { "grams", 0.001 },
                    { "pounds", 0.45359237 },
                    { "ounces", 0.028349523 }
                }
            }
        };

        private readonly HashSet<string> _temperatureUnits = new(StringComparer.OrdinalIgnoreCase)
        {
            "celsius", "fahrenheit", "kelvin"
        };

        public ConversionResponse Convert(ConversionRequest request)
        {
            string category = request.Category.ToLower();

            if (category == "temperature")
            {
                double convertedTemp = ConvertTemperature(request.FromUnit, request.ToUnit, request.Value);
                return BuildResponse(request, convertedTemp);
            }

            if (_linearUnits.TryGetValue(category, out var unitRegistry))
            {
                if (!unitRegistry.TryGetValue(request.FromUnit, out double fromFactor) ||
                    !unitRegistry.TryGetValue(request.ToUnit, out double toFactor))
                {
                    throw new ArgumentException($"Invalid unit combinations for category '{category}'.");
                }

                // Convert to base unit, then convert to target unit
                double valueInBaseUnit = request.Value * fromFactor;
                double convertedValue = valueInBaseUnit / toFactor;

                return BuildResponse(request, convertedValue);
            }

            throw new ArgumentException($"Category '{request.Category}' is not supported.");
        }

        public IEnumerable<string> GetSupportedCategories()
        {
            var categories = _linearUnits.Keys.ToList();
            categories.Add("temperature");
            return categories;
        }

        public IEnumerable<string> GetSupportedUnits(string category)
        {
            if (category.Equals("temperature", StringComparison.OrdinalIgnoreCase))
                return _temperatureUnits;

            if (_linearUnits.TryGetValue(category, out var units))
                return units.Keys;

            throw new ArgumentException($"Category '{category}' not found.");
        }

        private double ConvertTemperature(string from, string to, double value)
        {
            if (!_temperatureUnits.Contains(from) || !_temperatureUnits.Contains(to))
            {
                throw new ArgumentException("Unsupported temperature unit requested.");
            }

            if (from.Equals(to, StringComparison.OrdinalIgnoreCase)) return value;

            // Step 1: Normalize everything to Celsius first
            double celsius = from.ToLower() switch
            {
                "celsius" => value,
                "fahrenheit" => (value - 32) * 5 / 9,
                "kelvin" => value - 273.15,
                _ => throw new ArgumentException()
            };

            // Step 2: Convert from Celsius to Target Unit
            return to.ToLower() switch
            {
                "celsius" => celsius,
                "fahrenheit" => (celsius * 9 / 5) + 32,
                "kelvin" => celsius + 273.15,
                _ => throw new ArgumentException()
            };
        }

        private static ConversionResponse BuildResponse(ConversionRequest req, double results)
        {
            return new ConversionResponse
            {
                Category = req.Category,
                FromUnit = req.FromUnit,
                ToUnit = req.ToUnit,
                OriginalValue = req.Value,
                ConvertedValue = Math.Round(results, 6) // Smooth out floating-point precision quirks
            };
        }


    }
}
