using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp.Views.Maui.Controls;
//using Android.Runtime;

namespace app22
{
    // Informa ao linker para manter essa classe e seus membros
    [Preserve(AllMembers = true)]
    public class LinkerPreserve
    {
        public void Include()
        {
            // Cria instância do SKCanvasView para evitar remoção pelo linker
            var view = new SKCanvasView();
            view.IgnorePixelScaling = false;

            // Aqui você pode adicionar mais tipos que estão sendo removidos indevidamente
        }
    }
}
