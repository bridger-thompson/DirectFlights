using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DirectFlights.Shared
{
	public class TimeOnlyConverter : JsonConverter<TimeOnly>
	{
		private readonly string _timeFormat = "HH:mm:ss.FFFFFFF";
		public override TimeOnly Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options)
			=> TimeOnly.ParseExact(reader.GetString(), _timeFormat, CultureInfo.InvariantCulture);
		public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
			=> writer.WriteStringValue(value.ToString(_timeFormat, CultureInfo.InvariantCulture));
		
	}
}
