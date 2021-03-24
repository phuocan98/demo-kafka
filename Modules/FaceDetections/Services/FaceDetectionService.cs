using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Project.Kafka;
using Project.Modules.FaceDetections.Models;
using Project.Modules.FaceDetections.Requests;
using Project.Modules.UploadFiles.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Project.Modules.FaceDetections.Services
{
    public interface IFaceDetectionService
    {
        (RegisterFace data, string message) Register(RegisterRequest request);
        (DetectFace data, string message) Detect(RegisterRequest request);
        (DeleteFace data, string message) Delete(DeleteRequest request);
    }

    public class FaceDetectionService : IFaceDetectionService
    {
        public IConfiguration Configuration;
        private string Topic;
        private readonly KafkaDependentProducer<string, string> Producer;
        private readonly IUploadFileService UploadFileService;
        private readonly HandleTask<RegisterFace> HandleTaskRegister;
        private readonly HandleTask<DetectFace> HandleTaskDetect;
        private readonly HandleTask<DeleteFace> HandleTaskDelete;
        private readonly string App;
        private readonly string Bucket;

        public FaceDetectionService(IConfiguration configuration, KafkaDependentProducer<string, string> producer, IUploadFileService uploadFileService, HandleTask<RegisterFace> handleTask, HandleTask<DetectFace> handleTaskDetect, HandleTask<DeleteFace> handleTaskDelete)
        {
            Configuration = configuration;
            Producer = producer;
            UploadFileService = uploadFileService;
            HandleTaskRegister = handleTask;
            HandleTaskDetect = handleTaskDetect;
            HandleTaskDelete = handleTaskDelete;
            App = "TEST.wifi";
            Bucket = Configuration["OutsideSystems:AWS_S3:S3_BUCKET"];
        }

        public (RegisterFace data, string message) Register(RegisterRequest request)
        {
            var upload = UploadFileService.Upload(request.File, request.FolderPath).Result;
            if (!upload.check)
            {
                return (null, "An error occurred");
            }

            string objectName = upload.fullPath;
            JObject jObject = JObject.FromObject(new { bucket = Bucket, object_name = objectName, app = App });
            string key = $"{App}.{Bucket}.{objectName}";
            HandleTaskRegister.HandleTasks.Add(key, false);

            Topic = "REGISTER_FACE";
            Message<string, string> message = new Message<string, string> { Value = jObject.ToString() };
            this.Producer.Produce(Topic, message, deliveryReportHandleString);

            while (!HandleTaskRegister.Get(key))
            {
                Console.WriteLine("Register Task Running");
            }

            var data = HandleTaskRegister.GetData(key);
            HandleTaskRegister.Remove(key);

            if (data.Status != 200)
            {
                return (null, data.Message);
            }
            return (data, data.Message);
        }

        public (DetectFace data, string message) Detect(RegisterRequest request)
        {
            var upload = UploadFileService.Upload(request.File, request.FolderPath).Result;
            if (!upload.check)
            {
                return (null, "An error occurred");
            }

            string objectName = upload.fullPath;
            JObject jObject = JObject.FromObject(new { bucket = Bucket, object_name = objectName, app = App });
            string key = $"{App}.{Bucket}.{objectName}";
            HandleTaskDetect.HandleTasks.Add(key, false);

            Topic = "DETECT_FACE";
            Message<string, string> message = new Message<string, string> { Value = jObject.ToString() };
            this.Producer.Produce(Topic, message, deliveryReportHandleString);

            while (!HandleTaskDetect.Get(key))
            {
                Console.WriteLine("Detect Task Running");
            }

            var data = HandleTaskDetect.GetData(key);
            HandleTaskDetect.Remove(key);

            if (data.Status != 200)
            {
                return (null, data.Message);
            }
            return (data, data.Message);
        }

        public (DeleteFace data, string message) Delete(DeleteRequest request)
        {
            JObject jObject = JObject.FromObject(new { record = request.Record, id = request.FaceId, app = App });
            HandleTaskDelete.HandleTasks.Add(request.FaceId, false);
            Message<string, string> message = new Message<string, string> { Key = "data", Value = jObject.ToString() };
            Topic = "REMOVE_TOPIC";
            this.Producer.Produce(Topic, message, deliveryReportHandleString);

            while (!HandleTaskDelete.Get(request.FaceId))
            {
                Console.WriteLine("Delete Task Running");
            }

            var data = HandleTaskDelete.GetData(request.FaceId);
            HandleTaskDelete.Remove(request.FaceId);

            if (data.Status != 200)
            {
                return (null, data.Message);
            }
            return (data, data.Message);
        }


        private void deliveryReportHandleString(DeliveryReport<string, string> deliveryReport)
        {
            if (deliveryReport.Status == PersistenceStatus.NotPersisted)
            {
                Console.WriteLine($"Message delivery failed: {deliveryReport.Message.Value}");
            }
        }
    }
}
