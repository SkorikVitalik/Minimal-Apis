using System.Reflection;
using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;

namespace Minimal_Apis
{
    public record class Coordinate(double latilude, double longitude)
    {
        public static bool TryParse(string input, out Coordinate? coordinate)
        {
            coordinate = null;
            var splitArray = input.Split(',', 2);
            if (splitArray.Length != 2) return false;
            if (!double.TryParse(splitArray[0], out var lat)) return false;
            if (!double.TryParse(splitArray[1], out var longit)) return false;
            coordinate = new Coordinate(lat, longit);
            return true;
        }
        public static async ValueTask<Coordinate?> BindAsync(HttpContext context, ParameterInfo parameter)
        {
            return await Task<Coordinate?>.Run(() =>
            {
                var input = context.GetRouteValue(parameter.Name!) as string ?? string.Empty;
                TryParse(input, out var coordinate);
                return coordinate;
            });
        }
    };
}
