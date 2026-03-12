using System;
using MandalaLogics.SurfaceTerminal.Surfaces;
using MandalaLogics.SurfaceTerminal.Text;

namespace MandalaLogics.SurfaceTerminal.Layout
{
    public enum SurfaceLineState
    {
        None = 0,
        Selected,
        Disabled,
        Active,
        Enabled,
        Deactivated,
        Deselected
    }

    public delegate void SurfaceLineEventHandler(SurfaceLine sender);
    
    public abstract class SurfaceLine
    {
        public event SurfaceLineEventHandler? OnStateChanged;
        
        public SurfacePanel? Owner { get; internal set; }
        public SurfaceLineState State { get; private set; } = SurfaceLineState.None;
        
        public abstract void Render(ISurface<ConsoleChar> surface, ulong frameNumber);
        
        protected abstract bool StateChangeRequested(SurfaceLineState state);

        internal void KeyPressed(ConsoleKeyInfo keyInfo)
        {
            OnKeyPressed(keyInfo);
        }
        
        protected abstract void OnKeyPressed(ConsoleKeyInfo keyInfo);

        internal bool RequestStateChange(SurfaceLineState state)
        {
            if (StateChangeRequested(state))
            {
                State = state;
                
                OnStateChanged?.Invoke(this);
                
                return true;
            }
            else
            {
                return false;
            }
        }
        
        private bool TryChangeState(SurfaceLineState newState)
        {
            if (Owner is null) return false;

            if (Owner.LineStateTryChange(this, newState))
            {
                State = newState;
                
                OnStateChanged?.Invoke(this);
                
                return true;
            }
            else
            {
                return false;
            }
        }

        protected bool TrySelect() => TryChangeState(SurfaceLineState.Selected);

        protected bool TryActivate() => TryChangeState(SurfaceLineState.Active);
        
        protected bool TryEnable() => TryChangeState(SurfaceLineState.Enabled);

        protected bool TryDisable() => TryChangeState(SurfaceLineState.Disabled);

        protected bool TryDeactivate() => TryChangeState(SurfaceLineState.Deactivated);

        protected bool TryDeselect() => TryChangeState(SurfaceLineState.Deselected);
    }
}