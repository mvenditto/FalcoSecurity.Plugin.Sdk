using FalcoSecurity.Plugin.Sdk.Events;
using FalcoSecurity.Plugin.Sdk.Fields;
using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text.Json;
using System.Text;

namespace FalcoSecurity.Plugin.Sdk.DummyPlugin
{
    public class DockerEventSourceInstance: PushEventSourceInstance
    {
        private readonly CancellationTokenSource _cts = new();

        private readonly DockerClient _client;

        public DockerEventSourceInstance() : base(batchSize: 10)
        {
            _client = new DockerClientConfiguration()
                .CreateClient();

            _ = Task.Run(async () =>
            {
                var progress = new Progress<Message>();

                progress.ProgressChanged += async (_, msg) =>
                {
                    if (msg.Status == null) return;

                    var timestamp = (ulong) msg.TimeNano;

                    var json = JsonSerializer.Serialize(msg);

                    var data = Encoding.UTF8.GetBytes(json);

                    Console.WriteLine($"docker_event: {json}");

                    await EventsChannel.WriteAsync(new(timestamp, data), _cts.Token);
                };

                await _client.System.MonitorEventsAsync(
                    new(),
                    progress,
                    cancellationToken: _cts.Token);

                EventsChannel.Complete();

            }, _cts.Token);
        }

        public override void Dispose()
        {
            base.Dispose();
            _cts.Cancel();
            _client.Dispose(); 
        }
    }

    [FalcoPlugin(
        Id = 999,
        Name = "docker",
        Description = "docker Events",
        Contacts = "mvenditto",
        RequiredApiVersion = "2.0.0",
        Version = "1.0.0")]
    public class Plugin : PluginBase, IEventSource, IFieldExtractor
    {
        private ulong _lastEventNum = 0;
        private Message _lastDockerMessage;

        public string EventSourceName => "docker";

        public IEnumerable<string> EventSourcesToExtract
           => Enumerable.Empty<string>();

        public IEnumerable<OpenParam> OpenParameters
           => Enumerable.Empty<OpenParam>();

        public IEnumerable<ExtractionField> Fields => new List<ExtractionField>()
        {
            new(type: "string", name: "docker.status", desc: "Status of the event"),
            new(type: "string", name: "docker.id", desc: "ID of the event"),
            new(type: "string", name: "docker.from", desc: "From of the event new(deprecated)"),
            new(type: "string", name: "docker.type", desc: "type of the event"),
            new(type: "string", name: "docker.action", desc: "Action of the event"),
            new(type: "string", name: "docker.stack.namespace", desc: "Stack namespace"),
            new(type: "string", name: "docker.node.id", desc: "Swarm Node ID"),
            new(type: "string", name: "docker.swarm.task", desc: "Swarm Task"),
            new(type: "string", name: "docker.swarm.taskid", desc: "Swarm Task ID"),
            new(type: "string", name: "docker.swarm.taskname", desc: "Swarm Task name"),
            new(type: "string", name: "docker.swarm.servicename", desc: "Swarm Service name"),
            new(type: "string", name: "docker.node.statenew", desc: "Node New State"),
            new(type: "string", name: "docker.node.stateold", desc: "Node Old State"),
            new(type: "string", name: "docker.attributes.container", desc: "Attribute Container"),
            new(type: "string", name: "docker.attributes.image", desc: "Attribute Image"),
            new(type: "string", name: "docker.attributes.name", desc: "Attribute name"),
            new(type: "string", name: "docker.attributes.type", desc: "Attribute type"),
            new(type: "string", name: "docker.attributes.exitcode", desc: "Attribute Exit Code"),
            new(type: "string", name: "docker.attributes.signal", desc: "Attribute Signal"),
            new(type: "string", name: "docker.scope", desc: "Scope")
        };

        public void Close(IEventSourceInstance instance)
        {
            instance.Dispose();
        }

        public IEventSourceInstance Open(IEnumerable<OpenParam>? openParams)
        {
            return new DockerEventSourceInstance();
        }

        public void Extract(IExtractionRequest req, IEventReader evt)
        {
            if (evt.EventNum != _lastEventNum)
            {
                var jsonStr = Encoding.UTF8.GetString(evt.Data);
                var message = JsonSerializer.Deserialize<Message>(jsonStr);
                _lastEventNum = evt.EventNum;
                _lastDockerMessage = message;
            }

            var msg = _lastDockerMessage;

            switch(req.FieldName)
            {
                case "docker.status":
                    req.SetValue(msg.Status);
                    break;
                case "docker.id":
                    req.SetValue(msg.ID);
                    break;
                case "docker.from":
                    req.SetValue(msg.From);
                    break;
                case "docker.type":
                    req.SetValue(msg.Type);
                    break;
                case "docker.action":
                    req.SetValue(msg.Action);
                    break;
                case "docker.scope":
                    req.SetValue(msg.Scope);
                    break;
                case "docker.actor.id":
                    req.SetValue(msg.Actor.ID);
                    break;
                case "docker.stack.namespace":
                    req.SetValue(msg.Actor.Attributes["com.docker.stack.namespace"]);
                    break;
                case "docker.swarm.task":
                    req.SetValue(msg.Actor.Attributes["com.docker.swarm.task"]);
                    break;
                case "docker.swarm.taskid":
                    req.SetValue(msg.Actor.Attributes["com.docker.swarm.task.id"]);
                    break;
                case "docker.swarm.taskname":
                    req.SetValue(msg.Actor.Attributes["com.docker.swarm.task.name"]);
                    break;
                case "docker.swarm.servicename":
                    req.SetValue(msg.Actor.Attributes["com.docker.swarm.service.name"]);
                    break;
                case "docker.node.id":
                    req.SetValue(msg.Actor.Attributes["com.docker.swarm.node.id"]);
                    break;
                case "docker.node.statenew":
                    req.SetValue(msg.Actor.Attributes["state.new"]);
                    break;
                case "docker.node.stateold":
                    req.SetValue(msg.Actor.Attributes["state.old"]);
                    break;
                case "docker.attributes.container":
                    req.SetValue(msg.Actor.Attributes["container"]);
                    break;
                case "docker.attributes.image":
                    req.SetValue(msg.Actor.Attributes["image"]);
                    break;
                case "docker.attributes.name":
                    req.SetValue(msg.Actor.Attributes["name"]);
                    break;
                case "docker.attributes.type":
                    req.SetValue(msg.Actor.Attributes["type"]);
                    break;
                default: throw new ArgumentOutOfRangeException(
                        $"unknown field {req.FieldName}");
            };
        }
    }
}
