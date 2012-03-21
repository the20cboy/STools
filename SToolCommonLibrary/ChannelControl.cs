using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace STools.CommonLibrary
{
    public class Channel<T>
    {
        private string _name = string.Empty;
        private T _data;

        public string Name
        {
            get
            {
                return _name;
            }
        }

        public T Data
        {
            get { return _data; }
        }


        public Channel(string name, T data)
        {
            _name = name;
            _data = data;
        }

        public void SetValue(T data)
        {
            _data = data;
        }
    }
}
