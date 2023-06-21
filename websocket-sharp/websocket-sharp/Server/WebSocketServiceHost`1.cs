#region License
/*
 * WebSocketServiceHost`1.cs
 *
 * The MIT License
 *
 * Copyright (c) 2015-2017 sta.blockhead
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;
using AltWebSocketSharp.Net.WebSockets;

namespace AltWebSocketSharp.Server
{
    internal class WebSocketServiceHost<TBehavior> : WebSocketServiceHost
      where TBehavior : WebSocketBehavior
    {
        #region Private Fields

        private Func<WebSocketContext, TBehavior> _creator;

        #endregion

        #region Internal Constructors

        internal WebSocketServiceHost(
          string path, Func<TBehavior> creator, Logger log
        )
          : this(path, creator, null, log)
        {
        }

        internal WebSocketServiceHost(
          string path,
          Func<TBehavior> creator,
          Action<WebSocketContext, TBehavior> initializer,
          Logger log
        )
          : base(path, log)
        {
            _creator = createCreator(creator, initializer);
        }

        #endregion

        #region Public Properties

        public override Type BehaviorType
        {
            get
            {
                return typeof(TBehavior);
            }
        }

        #endregion

        #region Private Methods

        private Func<WebSocketContext, TBehavior> createCreator(
          Func<TBehavior> creator, Action<WebSocketContext, TBehavior> initializer
        )
        {
            return (context) =>
            {
                var ret = creator();
                if (initializer != null)
                    initializer(context, ret);

                return ret;
            };
        }

        #endregion

        #region Protected Methods

        protected override WebSocketBehavior CreateSession(WebSocketContext context)
        {
            return _creator(context);
        }

        #endregion
    }
}
