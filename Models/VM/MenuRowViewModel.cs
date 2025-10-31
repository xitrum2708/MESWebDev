using MESWebDev.Models.Master;

namespace MESWebDev.Models.VM
{
    public class MenuRowViewModel
    {
        public MenuViewModel Menu { get; set; }
        public FunctionModel Function { get; set; }
        public int Level { get; set; } = 0;
    }
}