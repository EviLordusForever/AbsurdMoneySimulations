using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AbsurdMoneySimulations
{

	public class LayerAbstractConverter : JsonConverter
	{
		static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassConverter() };

		public override bool CanConvert(Type objectType)
		{
			return (objectType == typeof(LayerAbstract));
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			//System.Text.Json.JsonSerializerOptions options = new System.Text.Json.JsonSerializerOptions(JsonSerializerDefaults.Web);
			reader.SupportMultipleContent = true;

			JObject jo = JObject.Load(reader);
			reader = jo.CreateReader();
			if (jo["type"] != null)
				switch (jo.GetValue("type").ToString())
				{
					case "0":
						return serializer.Deserialize(reader, typeof(LayerInput));
					case "1":
						return serializer.Deserialize(reader, typeof(LayerPerceptron));
					case "2":
						return serializer.Deserialize(reader, typeof(LayerMegatron));
					case "3":
						return serializer.Deserialize(reader, typeof(LayerCybertron));
					default:
						throw new Exception();
				}
			throw new NotImplementedException();
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException(); // won't be called because CanWrite returns false
		}
	}
}
