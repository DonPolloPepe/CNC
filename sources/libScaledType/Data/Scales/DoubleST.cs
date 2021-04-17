﻿using System;

namespace As.Tools.Data.Scales
{
    /// <summary>
    /// DoubleST: a long value with scale definition attached to it. (value, scale)
    /// </summary>
    public class DoubleST : ScaledType
    {
        /// <summary>
        /// Scan a string for a DoubleST value with no or a bracketless scale."
        /// </summary>
        /// <param name="value">Value to scan.</param>
        /// <param name="value_default">Default value to use if scan fails for the value part.</param>
        /// <param name="unit_default">Default scale to use if scan fails for the scale part.</param>
        /// <returns></returns>
        public static DoubleST Scan(string value, double value_default, string unit_default)
        {
            double V = value_default;
            string[] Va;
            if (string.IsNullOrWhiteSpace(value))
            {
                Va = new string[] { "", unit_default };
            }
            else
            {
                Va = value.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                switch (Va.Length)
                {
                    default:
                        Va = new string[] { "", unit_default };
                        break;
                    case 1:
                        Va = new string[] { Va[0], unit_default };
                        goto case 2;
                    case 2:
                        if (!double.TryParse(Va[0], out V)) V = value_default;
                        break;
                }
            }
            return new DoubleST(V, $"[{Va[1]}]");
        }

        /// <summary>
        /// Add operator +: Add two values with equal scale.
        /// </summary>
        /// <param name="left">A value to add to</param>
        /// <param name="right">A value to add</param>
        /// <exception cref="ArgumentOutOfRangeException">left scale is not (semantical) equal to right scale.</exception>
        /// <returns>(left.value + right.value, [scale])</returns>
        public static DoubleST operator +(DoubleST left, DoubleST right)
        {
            return Add(left, right);
        }

        /// <summary>
        /// Sub operator -: Subtract two values with equal scale.
        /// </summary>
        /// <param name="left">A value to subtract from</param>
        /// <param name="right">A value to subtract</param>
        /// <exception cref="ArgumentOutOfRangeException">left scale is not (semantical) equal to right scale.</exception>
        /// <returns>(left.value - right.value, [scale])</returns>
        public static DoubleST operator -(DoubleST left, DoubleST right)
        {
            return Sub(left, right);
        }

        /// <summary>
        /// Mul operator *: Multiply two values.
        /// </summary>
        /// <param name="left">A value to multiply with</param>
        /// <param name="right">A value to multiply</param>
        /// <returns>(left.value * right.value, [Normalised(left.scale right.scale)])</returns>
        public static DoubleST operator *(DoubleST left, DoubleST right)
        {
            return Mul(left, right);
        }

        /// <summary>
        /// Mul operator *: Multiply a value and a scalar.
        /// </summary>
        /// <param name="left">A value to multiply with</param>
        /// <param name="right">A scalar to multiply</param>
        /// <returns>(left.value * right, [left.scale])</returns>
        public static DoubleST operator *(DoubleST left, double right)
        {
            return new DoubleST(left.Value * right, left.Scale);
        }

        /// <summary>
        /// Mul operator *: Multiply a scalar and a value.
        /// </summary>
        /// <param name="left">A scalar to multiply with</param>
        /// <param name="right">A value to multiply</param>
        /// <returns>(left * right.value, [right.scale])</returns>
        public static DoubleST operator *(double left, DoubleST right)
        {
            return new DoubleST(left * right.Value, right.Scale);
        }

        /// <summary>
        /// Div operator /: Divide two values.
        /// </summary>
        /// <param name="left">A value to divide to</param>
        /// <param name="right">A value to divide with</param>
        /// <returns>(left.value / right.value, [Normalised(left.scale, right.scale^-1)])</returns>
        public static DoubleST operator /(DoubleST left, DoubleST right)
        {
            return Div(left, right);
        }

        /// <summary>
        /// Div operator /: Divide a value and a scalar.
        /// </summary>
        /// <param name="left">A value to divide to</param>
        /// <param name="right">A scalar to divide with</param>
        /// <returns>(left.value / right, [left.scale])</returns>
        public static DoubleST operator /(DoubleST left, double right)
        {
            return Div(left, new DoubleST(right));
        }

        /// <summary>
        /// Div operator /: Divide a scalar and a value.
        /// </summary>
        /// <param name="left">A scalar to divide to</param>
        /// <param name="right">A value to divide with</param>
        /// <returns>(left / right.value, [right.scale^-1])</returns>
        public static DoubleST operator /(double left, DoubleST right)
        {
            return Div(new DoubleST(left), right);
        }

        /// <summary>
        /// Add operator +: Add two values with equal scale.
        /// </summary>
        /// <param name="left">A value to add to</param>
        /// <param name="right">A value to add</param>
        /// <exception cref="ArgumentOutOfRangeException">left scale is not (semantical) equal to right scale.</exception>
        /// <returns>(left.value + right.value, [scale])</returns>
        public static DoubleST Add(DoubleST left, DoubleST right)
        {
            if (left.Scale != right.Scale)
            {
                var msg = $"DoubleST: operation not supported: {left.Scale} + {right.Scale}";
                //log.Debug(msg);
                throw new ArgumentOutOfRangeException(msg);
            }
            return new DoubleST(left.Value + right.Value, left.Scale);
        }

        /// <summary>
        /// Sub operator -: Subtract two values with equal scale.
        /// </summary>
        /// <param name="left">A value to subtract from</param>
        /// <param name="right">A value to subtract</param>
        /// <exception cref="ArgumentOutOfRangeException">left scale is not (semantical) equal to right scale.</exception>
        /// <returns>(left.value - right.value, [scale])</returns>
        public static DoubleST Sub(DoubleST left, DoubleST right)
        {
            if (left.Scale != right.Scale)
            {
                var msg = $"DoubleST: operation not supported: {left.Scale} - {right.Scale}";
                //log.Debug(msg);
                throw new ArgumentOutOfRangeException(msg);
            }
            return new DoubleST(left.Value - right.Value, left.Scale);
        }

        /// <summary>
        /// Mul operator *: Multiply two values.
        /// </summary>
        /// <param name="left">A value to multiply with</param>
        /// <param name="right">A value to multiply</param>
        /// <returns>(left.value * right.value, [Normalised(left.scale right.scale)])</returns>
        public static DoubleST Mul(DoubleST left, DoubleST right)
        {
            var result = new DoubleST(left.Value * right.Value, left.Scale);
            result.Append(right.Scale);
            return result;
        }

        /// <summary>
        /// Div operator /: Divide two values.
        /// </summary>
        /// <param name="left">A value to divide to</param>
        /// <param name="right">A value to divide with</param>
        /// <returns>(left.value / right.value, [Normalised(left.scale, right.scale^-1)])</returns>
        public static DoubleST Div(DoubleST left, DoubleST right)
        {
            if (right.Value == 0.0) throw new DivideByZeroException($"{left} / {right} ");
            var result = new DoubleST(left.Value / right.Value, left.Scale);
            result.Append(right.Scale, reciproce: true);
            return result;
        }

        /// <summary>
        /// .ctor: create a ScaledType instance from a given value and unit.
        /// </summary>
        /// <param name="value">The value of the ScaledType</param>
        /// <param name="unit">The scale of the ScaledType</param>
        public DoubleST(double value, Unit unit) : base(unit)
        {
            Value = value;
        }

        /// <summary>
        /// .ctor: create a ScaledType instance from a given balue and unit.
        /// </summary>
        /// <param name="value">The value of the ScaledType</param>
        /// <param name="scale">The scale of the ScaledType</param>
        /// <exception cref="ArgumentException">Cannot parse the scale</exception>
        public DoubleST(double value, string scale) : base(scale)
        {
            Value = value;
        }

        /// <summary>
        /// .ctor: create a ScaledType instance form a given (optional)value and (optional)scale.
        /// </summary>
        /// <param name="value">The value of the ScaledType, (default: 0.0)</param>
        /// <param name="scale">The scale of the ScaledType, (default: [#])</param>
        public DoubleST(double value = 0.0, Scale scale = null) : base(scale)
        {
            Value = value;
        }

        /// <summary>
        /// .ctor: copy constructor.
        /// </summary>
        /// <param name="other">Data to copy.</param>
        public DoubleST(DoubleST other) : base(other.Scale)
        {
            Value = other.value;
        }

        /// <summary>
        /// Duplicate: create a copy of this.
        /// </summary>
        /// <returns>Copy of this.</returns>
        public override ScaledType Duplicate()
        {
            return new DoubleST(this);
        }

        /// <summary>
        /// The value of a ScaledType instance.
        /// </summary>
        public double Value
        {
            get { return value; }
            set
            {
                if (this.value != value)
                {
                    this.value = value;
                    ValueChanged();
                }
            }
        }

        /// <summary>
        /// Container for Value.
        /// </summary>
        double value;

        /// <summary>
        /// IsFloatingPoint: indicator about the value type
        /// </summary>
        public override bool IsFloatingPoint { get { return true; } }

        /// <summary>
        /// Through the parent use of the value.
        /// </summary>
        /// <returns>The value as used by the child.</returns>
        public override object GetValue() { return Value; }

        /// <summary>
        /// SetValue: change to value from string content.
        /// </summary>
        /// <param name="value">string representation of a new value</param>
        /// <returns>True if the value is accepted by the child.</returns>
        public override bool SetValue(string value)
        {
            var result = double.TryParse(value, out double v);
            if (result) Value = v;
            return result;
        }

        /// <summary>
        /// SetValue: change to value from another ScaledType, no scaling used.
        /// </summary>
        /// <param name="value">ScaledType child from which the value should be used.</param>
        /// <returns>True if the value is accepted by the child.</returns>
        public override bool SetValue(ScaledType value)
        {
            if (ReferenceEquals(this, value)) return true;
            try
            {
                Value = (value is DoubleST)
                    ? (value as DoubleST).value
                    : (double)value.GetValue();
                return true;
            }
            catch (Exception x)
            {
                log.Debug($"DoubleST: Setvalue({value}), problem: {x}");
            }
            return false;
        }

        /// <summary>
        /// SetValue: change to value from another ScaledType, check for convertability of scales and rescale the new value.
        /// </summary>
        /// <param name="value">New value (possibly in different scale)</param>
        /// <returns>True if scales were convertable and a new value is set.</returns>
        public override bool SetValueScaled(ScaledType value)
        {
            if (ReferenceEquals(this, value)) return true;
            try
            {
                var v1 = (value is DoubleST)
                    ? value as DoubleST
                    : new DoubleST((double)value.GetValue(), value.GetScale());
                return SetValueScaled(v1, (DoubleST v2) =>
                {
                    Value = (v2 == null) ? v1.Value : v2.Value;
                });
            }
            catch (Exception x)
            {
                log.Debug($"DoubleST: SetvalueScaled({value}), problem: {x}");
            }
            return false;
        }

        /// <summary>
        /// Equals: is this scaled value (syntactical) equal to the other scaled value.
        /// </summary>
        /// <param name="obj">a scaled value to compare with</param>
        /// <returns>
        /// true if the values the scale from this are
        /// (syntactical) equal to the ones from the other
        /// </returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            if (!base.Equals(obj)) return false;
            var type = obj as DoubleST;
            return (type != null) && (Value == type.Value);
        }

        /// <summary>
        /// GetHashCode: Get the Hash code.
        /// </summary>
        /// <returns>The Hash value</returns>
        public override int GetHashCode()
        {
            var hashCode = base.GetHashCode();
            hashCode = hashCode * -1521134295 + Value.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Assign: assign a new value to this instance
        /// </summary>
        /// <param name="value">The new value</param>
        /// <remarks>It is not possible to override operator=()</remarks>
        public void Assign(double value)
        {
            Value = value;
        }

        /// <summary>
        /// Assign: assign a new value to this instance
        /// </summary>
        /// <param name="value">The new value</param>
        /// <exception cref="ArgumentOutOfRangeException">Scales of this and the other must be (syntactical) equivalents</exception>
        /// <remarks>It is not possible to override operator=()</remarks>
        public void Assign(DoubleST other)
        {
            if (ReferenceEquals(this, other)) return;
            base.Assign(other);
            Value = other.Value;
        }

        /// <summary>
        /// ToString: Readable representation of this (value [Scale]) instance.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Value} {Scale}";
        }
    }
}
