using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LabelPrint.Receipt
{

    public abstract class ReceiptPatternFactory
    {
        public abstract ReceiptPrintPattern CreateReceiptPrintPattern();
    }
}
