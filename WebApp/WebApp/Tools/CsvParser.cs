using Microsoft.VisualBasic.FileIO;
using WebApp.Models;
using WebApp.Pages;

namespace WebApp.Tools
{
    public static class CsvParser
    {
        public static bool TryParseCSV(Stream file, out List<Station> stations, out int invalidLines, params string[] delimiters)
        {
            invalidLines = 0;

            using (TextFieldParser parser = new TextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(delimiters);
                stations = new List<Station>();

                if (parser.EndOfData)
                {
                    return false;
                }

                //Check header length
                string[]? firstRow = parser.ReadFields();
                if (firstRow == null || firstRow.Length == 0)
                {
                    return false;
                }

                //Save first row length to make sure length doens't change in the file
                int headerFieldCount = firstRow.Length;

                //Check if first row is a header or a data row
                if (Station.TryBuildFromCSV(firstRow, out Station? stat))
                {
                    stations.Add(stat);
                }

                while (!parser.EndOfData)
                {
                    string[]? row = null;

                    //TODO: figure out how to check if ReadFields() can run.
                    //ReadFields() will throw an exception for every broken line, which can be somewhat expensive if there are a lot of them
                    try
                    {
                        row = parser.ReadFields();
                    }
                    catch
                    {
                        invalidLines++;
                    }

                    if (row != null
                        && row.Length == headerFieldCount
                        && Station.TryBuildFromCSV(row, out Station? station))
                    {
                        stations.Add(station);
                    }
                    else
                    {
                        invalidLines++;
                    }
                }

                return true;
            }
        }

        public static bool TryParseCSV(Stream file, out List<BikeTrip> trips, out int invalidLines, params string[] delimiters)
        {
            invalidLines = 0;

            using (TextFieldParser parser = new TextFieldParser(file))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(delimiters);
                trips = new List<BikeTrip>();

                if (parser.EndOfData)
                {
                    return false;
                }

                //Check header length
                string[]? firstRow = parser.ReadFields();
                if (firstRow == null || firstRow.Length == 0)
                {
                    return false;
                }

                //Save first row length to make sure length doens't change in the file
                int headerFieldCount = firstRow.Length;

                //Check if first row is a header or a data row
                if (BikeTrip.TryBuildFromCSV(firstRow, out BikeTrip? trp))
                {
                    trips.Add(trp);
                }

                while (!parser.EndOfData)
                {
                    string[]? row = null;

                    //TODO: figure out how to check if ReadFields() can run.
                    //ReadFields() will throw an exception for every broken line, which can be somewhat expensive if there are a lot of them
                    try
                    {
                        row = parser.ReadFields();
                    }
                    catch
                    {
                        invalidLines++;
                    }

                    if (row != null
                        && row.Length == headerFieldCount
                        && BikeTrip.TryBuildFromCSV(row, out BikeTrip? trip))
                    {
                        trips.Add(trip);
                    }
                    else
                    {
                        invalidLines++;
                    }
                }

                return true;
            }
        }
    }
}
