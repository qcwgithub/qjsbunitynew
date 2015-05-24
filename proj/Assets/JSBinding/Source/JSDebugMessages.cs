using SharpKit.JavaScript;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class JSDebugMessages
{

    #region Static variables

    private static List<Message> messages = new List<Message>();

    private const int startLeft = 5;
    private const int startTop = 20;
    private const int lineHeight = 20;

    #endregion

    #region Public methods

    public static void Add(string text)
    {
        for (int i = 0; i < messages.Count; i++)
        {

            var item = messages[i];

            var position = item.guiText.pixelOffset;
            position.y += lineHeight;

            item.guiText.pixelOffset = position;

        }

        var message = Message.Obtain();
        message.guiText.pixelOffset = new Vector2(startLeft, startTop);
        message.guiText.text = text;
        messages.Add(message);

    }

    #endregion

    #region Nested classes
    protected class Message
    {

        public GUIText guiText;

        #region Object pooling

        private static List<Message> pool = new List<Message>();

        public static Message Obtain()
        {

            if (pool.Count > 0)
            {

                var instance = pool[0];
                pool.RemoveAt(0);

                return instance;

            }

            var gameObject = new GameObject("__Message__");// { hideFlags = HideFlags.HideInHierarchy };

            var newInstance = new Message();
            newInstance.guiText = gameObject.AddComponent<GUIText>();
            return newInstance;
        }

        public void Release()
        {
            if (!pool.Contains(this))
            {
                messages.Remove(this);
                pool.Add(this);
            }
        }

        #endregion

    }

    #endregion

}
