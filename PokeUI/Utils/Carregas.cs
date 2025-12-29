using PokeData.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace PokeUI.Utils
{
    public static class Carregas
    {
        /// <summary>
        /// Mètode per carregar els tipus de pokémon dins del combobox i fer la configuració inicial.
        /// (Aquest mètode no és realment necessari però així quedava més entenedor i net.)
        /// </summary>
        public static void carregarTipus(AppDbContext context, ComboBox cb)
        {
            var llistaTipus = context.Types.ToList();
            cb.DisplayMemberPath = "TypeName";
            cb.SelectedValuePath = "TypeId";
            cb.ItemsSource = llistaTipus;
        }
    }
}
