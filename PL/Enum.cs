using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
namespace PL;

internal class Deliverytype : IEnumerable
{
    static readonly IEnumerable<BO.DeliveryType> s_enums =
  (Enum.GetValues(typeof(BO.DeliveryType)) as IEnumerable<BO.DeliveryType>)!;

    public IEnumerator GetEnumerator() => s_enums.GetEnumerator();
}

