﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Diagnostics;

namespace TreemapTest
{

    struct RowSlice
    {
        int _begin;
        int _end;

        public int Begin => _begin;
        public int End => _end;

        public RowSlice(int begin, int end)
        {
            _begin = begin;
            _end = end;
        }
    }

    public class Treemap
    {
        enum Orientation { Horizontal, Vertical }
//        const int margin = 1;
        double[] _sizes;
        Rect[] _rects;
        Rect _mainRect;

        RowSlice _curRow;
        Rect _curRect;
        double _curAreaSum, _curMin, _curMax, _shortSide, _shortSideSq;
        Orientation _curOrientation;
        int _layoutCount; // testing
        List<Rect> _usedRect; // testing

        public Treemap(double[] sizes, double x, double y, double width, double height)
        {
            //width -= margin * 2;
            //height -= margin * 2;
            _sizes = Normalize(sizes, width * height);
            _mainRect = new Rect(x, y, width, height);
            _curRect = new Rect(x, y, width, height);
            _usedRect = new List<Rect>(); //testing
            _usedRect.Add(_curRect);
            if (width >= height)
            {
                _curOrientation = Orientation.Vertical;
                _shortSideSq = height*height;
                _shortSide = height;
            }
            else
            {
                _curOrientation = Orientation.Horizontal;
                _shortSideSq = width*width;
                _shortSide = width;
            }
            _curAreaSum = 0;
            _curMin = double.MaxValue;
            _curMax = double.MinValue;
            _rects = new Rect[sizes.Length];
        }

        public void Squarify()
        {
            AddToCurrentRow(_sizes[0]);
            for (int i = 1, icnt = _sizes.Length-1; i < icnt; ++i)
            {
                double size = _sizes[i];
                if (ImprovesRatio(_curAreaSum, _curMin, _curMax, _shortSideSq, size))
                //if (Worst(_curAreaSum, _shortSideSq, _curRow.Begin, _curRow.End) >= Worst(_curAreaSum + size, _shortSideSq, _curRow.Begin, _curRow.End+1))
                {
                    AddToCurrentRow(size);
                }
                else
                {
                    LayoutRow();
                    AddToCurrentRow(size);
                }
            }
            AddToCurrentRow(_sizes[_sizes.Length - 1]);
            LayoutLastRow();
        }

        double[] Normalize(double[] sizes, double targetArea)
        {
            return sizes;
        }

        void AddToCurrentRow(double size)
        {
            _curRow = new RowSlice(_curRow.Begin, _curRow.End + 1);
            _curAreaSum += size;
            if (size > _curMax) _curMax = size;
            if (size < _curMin) _curMin = size;
        }

        bool ImprovesRatio(double sum, double min, double max, double sideLenSq, double nextSize)
        {
            double r1 = Ratio(sum, min, max, sideLenSq);
            sum += nextSize;
            if (min > nextSize) min = nextSize;
            if (max < nextSize) max = nextSize;
            double r2 = Ratio(sum, min, max, sideLenSq);
            return r1 >= r2;
        }


        double Worst(double sum, double sideLenSq, int begin, int end)
        {
            double max = double.MinValue;
            double sumsq = sum * sum;
            for (int i = begin; i < end; ++i)
            {
                double sz = _sizes[i];
                var m = Math.Max((sideLenSq * sz) / sumsq, sumsq / (sideLenSq * sz));
                if (max < m) max = m;
            }
            return max;
        }

        double Ratio(double sum, double min, double max, double sideLenSq)
        {
            double sumSq = sum * sum;
            double v1 = (sideLenSq * max) / sumSq;
            double v2 = sumSq / (sideLenSq * min);
            return Math.Max(v1, v2);
        }

        void LayoutLastRow()
        {
            double x = _curRect.X;
            double y = _curRect.Y;
            if (_curOrientation == Orientation.Vertical)
            {
                double width = _curAreaSum / _curRect.Height;
                for (int i = _curRow.Begin, icnt = _curRow.End; i < icnt; ++i)
                {
                    double area = _sizes[i];
                    double height = area / width;
                    _rects[i] = new Rect(x, y, width, height);
                    y += height;
                }
            }
            else
            {
                double height = _curAreaSum / _curRect.Width;
                for (int i = _curRow.Begin, icnt = _curRow.End; i < icnt; ++i)
                {
                    double area = _sizes[i];
                    double width = area / height;
                    _rects[i] = new Rect(x, y, width, height);
                    x += width;
                }
            }
            ++_layoutCount;
        }

        void LayoutRow()
        {
            double x = _curRect.X;
            double y = _curRect.Y;

            if (_curOrientation == Orientation.Vertical)
            {
                double width = _curAreaSum / _curRect.Height;
                for (int i = _curRow.Begin, icnt = _curRow.End; i < icnt; ++i)
                {
                    double area = _sizes[i];
                    double height = area / width;
                    _rects[i] = new Rect(x, y, width, height);
                    y += height;
                }
                _curRect = new Rect(x + width, _curRect.Y, _curRect.Width - width, _curRect.Height);
            }
            else
            {
                double height = _curAreaSum / _curRect.Width;
                for (int i = _curRow.Begin, icnt = _curRow.End; i < icnt; ++i)
                {
                    double area = _sizes[i];
                    double width = area / height;
                    _rects[i] = new Rect(x, y, width, height);
                    x += width;
                }
                _curRect = new Rect(_curRect.X, y+height, _curRect.Width, _curRect.Height-height);
            }
            _curAreaSum = 0;
            _curMax = double.MinValue;
            _curMin = double.MaxValue;
            _curRow = new RowSlice(_curRow.End, _curRow.End);
            if (_curRect.Width >= _curRect.Height)
            {
                _curOrientation = Orientation.Vertical;
                _shortSideSq = _curRect.Height * _curRect.Height;
            }
            else
            {
                _curOrientation = Orientation.Horizontal;
                _shortSideSq = _curRect.Width * _curRect.Width;
            }
            ++_layoutCount;
            _usedRect.Add(_curRect);
        }
    }
}
