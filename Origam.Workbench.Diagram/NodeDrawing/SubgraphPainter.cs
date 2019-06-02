using System;
using System.Drawing;
using System.Linq;
using Microsoft.Msagl.Core.Geometry.Curves;
using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Drawing;
using Origam.Workbench.Diagram.Extensions;
using Origam.Workbench.Diagram.Graphs;
using Node = Microsoft.Msagl.Drawing.Node;
using Point = Microsoft.Msagl.Core.Geometry.Point;

namespace Origam.Workbench.Diagram.NodeDrawing
{
    internal class SubgraphPainter : INodeItemPainter
    {
        private readonly InternalPainter painter;
        private readonly int emptySubgraphWidth = 200;
        private readonly int emptySubgraphHeight = 80;
        private readonly int imageGap = 10;

        public SubgraphPainter(InternalPainter internalPainter)
        {
            painter = internalPainter;
        }
        
        public ICurve GetBoundary(Node node) 
        {
            if (((BlockSubGraph) node).IsEmpty)
            {
                return CurveFactory.CreateRectangle(emptySubgraphWidth, emptySubgraphHeight, new Point());
            }
			
            var clusterBoundary = ((Cluster) node.GeometryNode).RectangularBoundary;

            var height = clusterBoundary.TopMargin;
            var labelWidth = painter.GetLabelWidth(node);

            var width = clusterBoundary.MinWidth > labelWidth
                ? clusterBoundary.MinWidth 
                : labelWidth + painter.LabelSideMargin * 2;

            return CurveFactory.CreateRectangle(width, height, new Point());
        }
        
        public bool Draw(Node node, object graphicsObj)
        {
            INodeData nodeData = (INodeData)node.UserData;
            var borderSize = new Size(
                (int)node.BoundingBox.Width,
                (int)node.BoundingBox.Height);

            Graphics editorGraphics = (Graphics)graphicsObj;

            var labelWidth = painter.GetLabelWidth(node);

            double centerX = node.GeometryNode.Center.X;
            double centerY = node.GeometryNode.Center.Y;
            var borderCorner = new System.Drawing.Point(
                (int)centerX - borderSize.Width / 2,
                (int)centerY - borderSize.Height / 2);
            Rectangle border = new Rectangle(borderCorner, borderSize);

            var labelPoint = new PointF(
                (float)(centerX - labelWidth / 2 + painter.ImageLeftMargin +  nodeData.PrimaryImage.Width + painter.ImageRightMargin),
                (float)centerY - border.Height / 2.0f + painter.LabelTopMargin);

            var imagePoint = new PointF(
                (float)(centerX - labelWidth / 2 + painter.ImageLeftMargin),
                (float)(centerY - border.Height / 2.0f + painter.ImageTopMargin));

            Rectangle imageBackground = new Rectangle(
                borderCorner,
                new Size(border.Width, painter.HeadingBackgroundHeight));
            
            var secondaryImagePoint = new PointF(
                imagePoint.X - nodeData.SecondaryImage?.Width ?? 0 - imageGap, 
                imagePoint.Y);

            var (emptyMessagePoint, emptyGraphMessage) = GetEmptyNodeMessage(node);
			
            editorGraphics.DrawUpSideDown(drawAction: graphics =>
                {
                    graphics.FillRectangle(painter.LightGreyBrush, imageBackground);
                    graphics.DrawString(node.LabelText, painter.Font,
                        painter.GetTextBrush(nodeData.IsFromActivePackage),
                        labelPoint, painter.DrawFormat);
                    if (!string.IsNullOrWhiteSpace(emptyGraphMessage))
                    {
                        graphics.DrawString(emptyGraphMessage, painter.Font,
                            painter.BlackBrush, emptyMessagePoint, painter.DrawFormat);
                    }
                    graphics.DrawRectangle(painter.GetActiveBorderPen(node), border);
                    graphics.DrawImage(nodeData.PrimaryImage, imagePoint);
                    if (nodeData.SecondaryImage != null)
                    {
                        graphics.DrawImage(nodeData.SecondaryImage, secondaryImagePoint);
                    }
                },
                yAxisCoordinate: (float)node.GeometryNode.Center.Y);

            return true;
        }
        
        private Tuple<PointF, string> GetEmptyNodeMessage(Node node)
        {
            double centerX = node.GeometryNode.Center.X;
            double centerY = node.GeometryNode.Center.Y;
			
            if (!((BlockSubGraph) node).IsEmpty)
            {
                return new Tuple<PointF, string>(new PointF(), "");
            }

            string emptyGraphMessage = "Right click here to add steps";
            SizeF messageSize = painter.MeasureString(emptyGraphMessage);
            var emptyMessagePoint = new PointF(
                (float)centerX -  messageSize.Width / 2,
                (float)centerY + painter.HeadingBackgroundHeight / 2 - messageSize.Height / 2 );
			
            return new Tuple<PointF, string>(emptyMessagePoint, emptyGraphMessage);
        }
    }
}