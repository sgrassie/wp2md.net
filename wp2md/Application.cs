using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp2md
{
    class Application
    {
        private Options _options;
        private Action<string> _logger;

        public Application(Action<string> logger)
        {
            _logger = logger;
        }

        public void Execute(Options options)
        {
            _options = options;
        }
    }
}
