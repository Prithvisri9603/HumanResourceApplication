using Azure.Messaging.EventGrid;
using Azure;
using Newtonsoft.Json;
using HumanResourceApplication.DTO;

namespace HumanResourceApplication.Utility
{
    public class Helper
    {
        public static async Task PublishToEventGrid(IConfiguration configuration, EmployeeDTO employeeDTO)
        {

            var endPoint = configuration.GetValue<string>("EventGridTopicEndpoint");
            var accessKey = configuration.GetValue<string>("EventGridAccessKey");

            EventGridPublisherClient client = new EventGridPublisherClient(new Uri(endPoint), new AzureKeyCredential(accessKey));

            var event1 = new EventGridEvent("PMS", "PMS.SupplierEvent", "1.0", JsonConvert.SerializeObject(employeeDTO));
            event1.Id = (new Guid()).ToString();
            event1.EventTime = DateTime.Now;
            event1.Topic = configuration.GetValue<string>("EventGridSubscription");

            List<EventGridEvent> eventList = new List<EventGridEvent> { event1 };

            await client.SendEventsAsync(eventList);


        }
    }
}
