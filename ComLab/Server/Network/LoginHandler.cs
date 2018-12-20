using System;
using System.Linq;
using System.Net;
using ComLab.ViewModels;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;

namespace ComLab.Network
{
    partial class Server
    {
        private async void LoginHandler(PacketHeader packetheader, Connection connection, Login login)
        {
            var result = new LoginResult();
            try
            {
                if (!MainViewModel.Instance.IsAuthenticated)
                {
                    result.Success = false;
                    result.Message = "PLEASE WAIT FOR YOUR INSTRUCTOR";
                    await result.Send(connection.ConnectionInfo.RemoteEndPoint as IPEndPoint);
                    return;
                }

                if (MainViewModel.Instance.ClassSession == null)
                {
                    result.Success = false;
                    result.Message = "CLASS HAS NOT YET STARTED";
                    await result.Send(connection.ConnectionInfo.RemoteEndPoint as IPEndPoint);
                    return;
                }

                var terminal = Clients.FirstOrDefault(x => x.IP == ((IPEndPoint)connection.ConnectionInfo.RemoteEndPoint).Address.ToString());
                if (terminal == null)
                {
                    result.Success = false;
                    result.Message = "UNRECOGNIZED TERMINAL";
                    await result.Send(connection.ConnectionInfo.RemoteEndPoint as IPEndPoint);
                    return;
                }

                var student = terminal.Student;
                if (student == null)
                    student = Students.Cache.FirstOrDefault(x => x.Username.ToLower() == login.Username.ToLower() ||
                                                                 x.StudentId.ToLower() == login.Username.ToLower());

                if (student == null)
                {
                    result.Success = false;
                    result.Message = "INVALID USERNAME AND PASSWORD!";
                    await result.Send(connection.ConnectionInfo.RemoteEndPoint as IPEndPoint);
                    return;
                }

                if (string.IsNullOrEmpty(student.Password)) student.Update(nameof(student.Password), login.Password);

                if (student.Password != login.Password)
                {
                    result.Message = "INVALID USERNAME AND PASSWORD!";
                    result.Success = false;
                    await result.Send(connection.ConnectionInfo.RemoteEndPoint as IPEndPoint);
                    return;
                }

                terminal.Student = student;
                result.Student = new Network.Student
                {
                    Firstname = student.Firstname,
                    Lastname = student.Lastname,
                    Course = student.Course,
                };

                result.Success = true;
                result.Message = "https://github.com/awooo-ph/comlab";

                await result.Send(connection.ConnectionInfo.RemoteEndPoint as IPEndPoint);
            }
            catch (Exception e)
            {
                result.Success = false;
                result.Message = e.Message;
            }

            await result.Send(connection.ConnectionInfo.RemoteEndPoint as IPEndPoint);
        }

    }
}
