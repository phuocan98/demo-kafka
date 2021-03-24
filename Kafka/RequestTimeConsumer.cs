using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Modules.FaceDetections.Models;
using Project.Modules.FaceDetections.Services;

namespace Project.Kafka
{
    public class RequestTimeConsumer : BackgroundService
    {
        private readonly IConsumer<string, string> kafkaConsumer;
        private readonly IConfiguration Config;
        private readonly IEnumerable<string> Channels;
        private readonly HandleTask<RegisterFace> HandleTaskRegister;
        private readonly HandleTask<DetectFace> HandleTaskDetect;
        private readonly HandleTask<DeleteFace> HandleTaskDelete;
        public RequestTimeConsumer(IConfiguration config, HandleTask<RegisterFace> handleTaskRegister, HandleTask<DetectFace> handleTaskDetect, HandleTask<DeleteFace> handleTaskDelete)
        {
            Config = config;
            var consumerConfig = new ConsumerConfig()
            {
                GroupId = Config["OutsideSystems:Kafka:ConsumerSettings:GroupId"],
                BootstrapServers = Config["OutsideSystems:Kafka:ConsumerSettings:BootstrapServers"]
            };
            this.kafkaConsumer = new ConsumerBuilder<string, string>(consumerConfig).Build();

            //get list kenh dang ky
            this.Channels = config.GetSection("OutsideSystems:Kafka:AllowedSucribers").Get<IEnumerable<string>>();
            Console.WriteLine(JsonConvert.SerializeObject(Channels));
            HandleTaskRegister = handleTaskRegister;
            HandleTaskDetect = handleTaskDetect;
            HandleTaskDelete = handleTaskDelete;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            new Thread(() => StartConsumerLoop(stoppingToken)).Start();

            return Task.CompletedTask;
        }

        private void StartConsumerLoop(CancellationToken cancellationToken)
        {
            //subcribe duoc nhieu keu
            kafkaConsumer.Subscribe(Channels);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var cr = this.kafkaConsumer.Consume(cancellationToken);
                    if (cr.Topic.Equals("REGISTER_FACE_RESPONSE"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("REGISTER_FACE_RESPONSE --- " + cr.Message.Value);
                        Console.ForegroundColor = ConsoleColor.White;
                        RegisterFace registerFace = new RegisterFace();
                        registerFace = JsonConvert.DeserializeObject<RegisterFace>(cr.Message.Value);
                        HandleTaskRegister.Action(registerFace.Record, registerFace);
                    }
                    if (cr.Topic.Equals("DETECT_FACE_RESPONSE"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("DETECT_FACE_RESPONSE --- " + cr.Message.Value);
                        Console.ForegroundColor = ConsoleColor.White;
                        DetectFace detectFace = new DetectFace();
                        detectFace = JsonConvert.DeserializeObject<DetectFace>(cr.Message.Value);
                        HandleTaskDetect.Action(detectFace.Record, detectFace);
                    }
                    if (cr.Topic.Equals("REMOVE_TOPIC_RESPONSE"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("REMOVE_TOPIC_RESPONSE --- " + cr.Message.Value);
                        Console.ForegroundColor = ConsoleColor.White;
                        DeleteFace deleteFace = new DeleteFace();
                        deleteFace = JsonConvert.DeserializeObject<DeleteFace>(cr.Message.Value);
                        HandleTaskDelete.Action(deleteFace.Id, deleteFace);
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (ConsumeException e)
                {
                    // Consumer errors should generally be ignored (or logged) unless fatal.
                    Console.WriteLine($"Consume error: {e.Error.Reason}");

                    if (e.Error.IsFatal)
                    {
                        break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Unexpected error: {e}");
                    break;
                }
            }
        }

        public override void Dispose()
        {
            this.kafkaConsumer.Close(); // Commit offsets and leave the group cleanly.
            this.kafkaConsumer.Dispose();

            base.Dispose();
        }
    }
}
