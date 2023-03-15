using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Windows.Forms;

UdpClient server = new UdpClient(45678);

while (true)
{

    var result = await server.ReceiveAsync();

    new Task(async () =>
    {

        var remoteEP = result.RemoteEndPoint;
        while (true)
        {
            await Task.Delay(35);
            var img = TakeScreenShot();

            var bytesImg = ImageToByte(img);


            var myArray = bytesImg.Chunk(ushort.MaxValue - 29);

            foreach (var array in myArray)
                await server.SendAsync(array, array.Length, remoteEP);
        }

    }).Start();
}

byte[] ImageToByte(Image img)
{
    using (var stream = new MemoryStream())
    {
        img.Save(stream, System.Drawing.Imaging.ImageFormat.Jpeg);
        return stream.ToArray();
    }
}


Image TakeScreenShot()
{
    Bitmap memoryImage;
    var width = Screen.PrimaryScreen.Bounds.Width;
    var height = Screen.PrimaryScreen.Bounds.Height;

    memoryImage = new Bitmap(width, height);

    Graphics memoryGraphics = Graphics.FromImage(memoryImage);
    memoryGraphics.CopyFromScreen(0, 0, 0, 0, memoryImage.Size);

    return memoryImage;
}