//using Microsoft.Azure.ServiceBus;
//using Microsoft.Azure.ServiceBus.Core;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace BasicSendReceiveTutorialWithFilters
{
    internal class SendReceive
    {
        public int NrOfMessagesPerStore;
        public string ServiceBusConnectionString;
        public string[] Store;
        public string[] Subscriptions;
        public string TopicName;

        public async Task SendMessages()
        {
            try
            {
                await using var client = new ServiceBusClient(ServiceBusConnectionString);
                var taskList = Store.Select(t => SendItems(client, t)).ToList();
                await Task.WhenAll(taskList);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.WriteLine("\nAll messages sent.\n");
        }

        private async Task SendItems(ServiceBusClient client, string store)
        {
            // create the sender
            var tc = client.CreateSender(TopicName);
            for (var i = 0; i < NrOfMessagesPerStore; i++)
            {
                var r = new Random();
                var item = new Item(r.Next(5), r.Next(5), r.Next(5));
                // Note the extension class which is serializing an deserializing messages
                var message = item.AsMessage();
                message.To = store;
                message.ApplicationProperties.Add("StoreId", store);
                message.ApplicationProperties.Add("Price", item.GetPrice().ToString());
                message.ApplicationProperties.Add("Color", item.GetColor());
                message.ApplicationProperties.Add("Category", item.GetItemCategory());
                await tc.SendMessageAsync(message);
                Console.WriteLine(
                    $"Sent item to Store {store}. Price={item.GetPrice()}, Color={item.GetColor()}, Category={item.GetItemCategory()}");
                ;
            }
        }

        public async Task Receive()
        {
            var taskList = new List<Task>();
            for (var i = 0; i < Subscriptions.Length; i++) taskList.Add(ReceiveMessages(i.ToString()));
            await Task.WhenAll(taskList);
        }

        private async Task ReceiveMessages(string subscription)
        {
            await using var client = new ServiceBusClient(ServiceBusConnectionString);
            var receiver = client.CreateReceiver(TopicName, subscription);

            // In reality you would not break out of the loop like in this example but would keep looping. The receiver keeps the connection open
            // to the broker for the specified amount of seconds and the broker returns messages as soon as they arrive. The client then initiates
            // a new connection. So in reality you would not want to break out of the loop. 
            // Also note that the code shows how to batch receive, which you would do for performance reasons. For convenience you can also always
            // use the regular receive pump which we show in our Quick Start and in other github samples.
            while (true)
                try
                {
                    //IList<Message> messages = await receiver.ReceiveAsync(10, TimeSpan.FromSeconds(2));
                    // Note the extension class which is serializing an deserializing messages and testing messages is null or 0.
                    // If you think you did not receive all messages, just press M and receive again via the menu.
                    var messages = await receiver.ReceiveMessagesAsync(100);

                    if (messages.Any())
                        foreach (var message in messages)
                        {
                            lock (Console.Out)
                            {
                                var item = message.As<Item>();
                                var myApplicationProperties = message.ApplicationProperties;
                                Console.WriteLine($"StoreId={myApplicationProperties["StoreId"]}");
                                if (message.Subject != null) Console.WriteLine($"Subject={message.Subject}");
                                Console.WriteLine(
                                    $"Item data: Price={item.GetPrice()}, Color={item.GetColor()}, Category={item.GetItemCategory()}");
                            }

                            await receiver.CompleteMessageAsync(message);
                        }
                    else
                        break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
        }
    }
}