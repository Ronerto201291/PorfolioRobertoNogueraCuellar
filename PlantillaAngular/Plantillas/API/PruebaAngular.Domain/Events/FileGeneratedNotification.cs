using MediatR;
using System.Collections.Generic;

namespace PruebaAngular.Domain.Events
{
    public class FileGeneratedNotification : INotification
    {
        public string Path { get; set; }
        public string TypeOfDocument { get; set; }
        public string Model { get; set; }
        public List<string> To { get; set; }
    }
}
