using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace ComLab.Network
{
    [ProtoContract]
    class StudentInfo:Packet<StudentInfo>
    {
        public StudentInfo() { }
        
        private string _Firstname;
        [ProtoMember(1)]
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
        [ProtoMember(2)]
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
        [ProtoMember(3)]
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
