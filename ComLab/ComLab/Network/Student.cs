using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace ComLab.Network
{
    [ProtoContract]
    class Student:Packet<Student>
    {
        public Student() { }

        private string _Firstname;

        public string Firstname
        {
            get => _Firstname;
            set
            {
                if (value == _Firstname) return;
                _Firstname = value;
                OnPropertyChanged(nameof(Firstname));
            }
        }

        private string _Lastname;

        public string Lastname
        {
            get => _Lastname;
            set
            {
                if (value == _Lastname) return;
                _Lastname = value;
                OnPropertyChanged(nameof(Lastname));
            }
        }

        private string _Course;

        public string Course
        {
            get => _Course;
            set
            {
                if (value == _Course) return;
                _Course = value;
                OnPropertyChanged(nameof(Course));
            }
        }


    }
}
