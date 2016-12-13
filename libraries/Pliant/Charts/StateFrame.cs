using Pliant.Grammars;
using Pliant.Utilities;

namespace Pliant.Charts
{
    public class StateFrame
    {
        public Frame Frame { get; private set; }

        public int Origin { get; private set; }

        private readonly int _hashCode;
                
        public StateFrame(Frame frame, int origin)
        {
            Frame = frame;
            Origin = origin;
            
            _hashCode = ComputeHashCode(Frame, Origin);  
        }
        
        public override bool Equals(object obj)
        {
            if (((object)obj) == null)
                return false;
            var stateFrame = obj as StateFrame;
            if (((object)stateFrame) == null)
                return false;
            return stateFrame.Origin == Origin && Frame.Equals(stateFrame.Frame);
        }

        public override int GetHashCode()
        {
            return _hashCode;
        }
        
        private static int ComputeHashCode(Frame frame)
        {
            return frame.GetHashCode();
        }

        private static int ComputeHashCode(Frame frame, int origin)
        {
            return HashCode.Compute(frame.GetHashCode(), origin.GetHashCode());
        }
    }
}
