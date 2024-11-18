using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF.Fake.mq.rabbitmqfake
{
    public interface IConsume
    {
        void Consume(object message);

    }
}
