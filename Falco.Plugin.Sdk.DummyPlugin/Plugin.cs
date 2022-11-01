using Falco.Plugin.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.Json;

namespace Falco.Plugin.Sdk
{
    [FalcoPlugin(
        Id = 42,
        Name = "DummyPlugin",
        Description = "A dummy plugin",
        Contacts = "mvenditto",
        RequiredApiVersion = "2.0.0",
        Version = "1.0.0")]
    public class Plugin : IEventSource
    {
        public string EventSourceName => "dummy_source";

        public IList<string> EventSourcesToConsume => new List<string> 
        { 
            "some_source_1",
            "some_source_2"
        };

        public IList<OpenParam> OpenParameters => new List<OpenParam>
        {
            new(value: "resource1", desc: "An example of openable resource"),
            new(value: "resource2", desc: "Another example of openable resource"),
            new(value: "res1;res2;res3", desc: "Some names", separator: ";"),
        };
    }
}
