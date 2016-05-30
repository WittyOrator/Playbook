using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Windows.Forms;

namespace Webb.Playbook
{
    public class MessageFilter : IMessageFilter
    {
        public virtual bool PreFilterMessage(ref Message m)
        {
            ////if里根据自己的需要进行判断判断 m.HWnd为控件句柄  m.Msg为消息ID            
            //if (false)
            //{
            //    //返回值为true， 表示消息已被处理，不要再往后传递，因此消息被截获                 
            //    return true;
            //}

            switch (m.Msg)
            {
                case (int)Webb.Playbook.Data.MessageCommands.WM_KEYDOWN:
                    if (Presentation.DrawingWindow.OriDrawing != null && Webb.Playbook.Geometry.Behavior.DrawVideo)
                    {
                        char key = (char)m.WParam;

                        if (key == '.')
                        {
                            Presentation.DrawingWindow.OriDrawing.Delete();
                        }
                    }
                    break;
            }

            //返回值为false，表示消息未被处理，需要再往后传递，因此消息未被截获             
            return false;
        }

        public void AddThisMessageFilter()
        {
            //添加该消息筛选器            
            System.Windows.Forms.Application.AddMessageFilter(this);
        }

        public void RemoveThisMessageFilter()
        {
            //移除该消息筛选器            
            System.Windows.Forms.Application.RemoveMessageFilter(this);
        }
    }
}
