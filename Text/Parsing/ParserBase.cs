using System;
using System.IO;
using System.Text;

namespace Libs.Text.Parsing
{
    public abstract class ParserBase
    {
        private TextReader m_Reader;
        private int m_State = -2;

        protected bool State
        {
            get { return m_State != -1; }
        }

        protected int Position { get; private set; } = -1;

        protected char Current
        {
            get
            {
                if(!State)
                    throw new System.IndexOutOfRangeException();

                return (char)m_State;
            }
        }

        protected void Next()
        {
            if(!State)
                throw new System.IndexOutOfRangeException();

            m_State = m_Reader.Read();
            Position++;
        }

        protected string Next(int count)
        {
            StringBuilder result = new StringBuilder();
            for(int i = 0; State && i < count; i++)
            {
                result.Append(Current);
                Next();
            }

            return result.ToString();
        }

        protected string Next(Predicate<char> predicate)
        {
            StringBuilder result = new StringBuilder();
            while(State && predicate(Current))
            {
                result.Append(Current);
                Next();
            }

            return result.ToString();
        }

        protected int Skip(int count)
        {
            int counter = 0;
            for(int i = 0; State && i < count; i++)
            {
                Next();
                counter++;
            }

            return counter;
        }

        protected int Skip(Predicate<char> predicate)
        {
            int counter = 0;
            while(State && predicate(Current))
            {
                Next();
                counter++;
            }

            return counter;
        }

        protected void Begin(TextReader reader)
        {
            m_Reader = reader;
            m_State = -2;
            Position = -1;
            Next();
        }

        protected void Close()
        {
            if(m_Reader == null)
                return;

            m_Reader = null;
            m_State = -1;
            Position = -1;
        }
    }
}
