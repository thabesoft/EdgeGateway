using System.Text.Json.Nodes;
using ThabeSoft.EdgeGateway.Configuration.Json;
using ThabeSoft.Guard;
using ThabeSoft.EdgeGateway.Configuration;

namespace ThabeSoft.EdgeGateway.UnitTests.Configuration;


[TestClass]
public class JsonConfigNodeTests
{
    [TestMethod]
    public void TestMethod1()
    {
        var json = JsonNode.Parse("""
            {
                "Obj" : {
                    "Str" : "Str",
                    "Number": 3.14
                },
                "Arr" :[
                    1, 2, 3, 4
                ]
            }
            """).Required();

        var node = new JsonConfigNode(json);


        Assert.AreEqual("Str", node["Obj"]?["Str"]?.AsString());
        Assert.AreEqual("3.14", node["Obj"]?["Number"]?.AsString());



        var a = node["Obj"]["Str"];
        var b = a.GetParent;
        Assert.AreEqual("Str", a?.AsString());
    }
}
