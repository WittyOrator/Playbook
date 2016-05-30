using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Effects;

namespace Webb.Playbook.Geometry
{
    public class PBEffects
    {
        static PBEffects()
        {
            DropShadowEffect dse = new DropShadowEffect()
            {
                BlurRadius = 0,
                ShadowDepth = 2,
            };

            SelectedEffect = dse;
        }

        public static Effect SelectedEffect = new DropShadowEffect();
    }
}
