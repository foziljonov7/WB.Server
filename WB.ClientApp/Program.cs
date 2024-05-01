using System.Net.WebSockets;
using System.Text;

Console.WriteLine("Press enter to cont.......");
Console.ReadLine();

using(ClientWebSocket client = new ClientWebSocket())
{
	Uri serviceUri = new Uri("ws://localhost:5178/send");
	var cTs = new CancellationTokenSource();
	cTs.CancelAfter(TimeSpan.FromSeconds(120));

	try
	{
		await client.ConnectAsync(serviceUri, cTs.Token);
		var n = 0;
		while(client.State == WebSocketState.Open)
		{
            Console.WriteLine("enter message to send....");
			string message = Console.ReadLine();

			if(!string.IsNullOrEmpty(message))
			{
				ArraySegment<byte> byteToSend = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
				await client.SendAsync(byteToSend, WebSocketMessageType.Text, true, cTs.Token);

				var responseBuffer = new byte[1024];
				var offset = 0;
				var packet = 1024;

				while(true)
				{
					ArraySegment<byte> byteReceived = new ArraySegment<byte>(responseBuffer, offset, packet);
					WebSocketReceiveResult response = await client.ReceiveAsync(byteReceived, cTs.Token);

					var responseMessage = Encoding.UTF8.GetString(responseBuffer, offset, response.Count);
                    Console.WriteLine(responseMessage);
					if (response.EndOfMessage)
						break;
                }
			}

		}
	}
	catch(WebSocketException ex)
	{
		Console.WriteLine(ex.Message);
	}
}

Console.ReadLine();