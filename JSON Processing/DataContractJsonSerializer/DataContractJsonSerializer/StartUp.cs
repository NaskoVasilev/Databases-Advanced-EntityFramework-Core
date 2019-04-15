using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Linq;
using System.Xml;

namespace CSharpDataContractJsonSerializer
{
    class StartUp
    {
        static void Main(string[] args)
        {
            ConvertJsonToStudent();
        }

        public static void ConvertJsonToStudent()
        {
            string json = @"{""Age"":17,""Class"":11,""FirstName"":""Atanas"",""Grades"":[6,6,5,6],""LastName"":""Vasilev""}";

            Student student = JsonConvert.DeserializeObject<Student>(json);

            Console.WriteLine(JsonConvert.SerializeObject(student));
        }
    }
}
