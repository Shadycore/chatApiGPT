using System;
using System.IO;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

class Chatapigpt
{
    static void Main(string[] args)
    {

        //tomo los datos de la configuración
        var config = JObject.Parse(File.ReadAllText("config.json"));
        var apiKey = config["api_key"].ToString();
        var model = config["model"].ToString();
        var maxTokens = config["max_tokens"];
        var temperature = config["temperature"];

        //Conexión a la api de GPT 3
        var client = new RestClient("https://api.openai.com/v1");
        var request = new RestRequest("/completions", Method.Post);

        // Aquí debe agregar las credenciales de su API de GPT-3
        request.AddHeader("Authorization", "Bearer " + apiKey);
        request.AddHeader("Content-Type", "application/json");
        Console.WriteLine("Para terminar de interacturar escribir: SALIR \n");
        Console.WriteLine("\u001b[37mYo: "); //Color blanco para las preguntas
        var texto = Console.ReadLine();
        while (texto != "SALIR")
        {
            // Crea el objeto de la solicitud
            var requestObject = new { model = model, prompt = texto, max_tokens = maxTokens, temperature = temperature };
            // Convierte el objeto en una cadena JSON
            var jsonString = JsonConvert.SerializeObject(requestObject);
            // Agrega la cadena JSON como parámetro a la solicitud
            request.AddParameter("application/json", jsonString, ParameterType.RequestBody);

            // Envía la solicitud a la API
            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Imprime la respuesta de GPT-3 
                var data = JObject.Parse(response.Content);
                var choices = data["choices"].First;
                var textgpt = choices["text"].ToString();
                Console.WriteLine("\u001b[32mGPT-3: " + textgpt); //color verde para las respuestas
            }
            else
            {
                Console.WriteLine("Error al conectarse a la API de GPT-3: " + response.Content);
            }

            // Espera la entrada del usuario para el siguiente mensaje
            Console.Write("\u001b[37mYo: ");
            texto = Console.ReadLine();
        }


    }
}