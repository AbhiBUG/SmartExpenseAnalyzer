using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SmartExpenseAnalyzer.UI
{
    // A Panel that looks and behaves like a mini floating form inside MainForm
    public class MiniFormPanel : Panel
    {
        private Panel _titleBar;
        private Label _titleLabel;
        private Button _btnClose;
        private Button _btnMinimize;
        private Panel _contentArea;

        private bool _isMinimized = false;
        private int _expandedHeight;

        // Drag support
        private bool _dragging = false;
        private Point _dragStart;

        public Panel ContentArea => _contentArea; // Add your controls here

        public MiniFormPanel(string title, int x, int y, int width, int height)
        {
            // ── Outer panel (the "window") ────────────────────────────
            this.Location = new Point(x, y);
            this.Size = new Size(width, height);
            this.BackColor = Color.White;
            this.BorderStyle = BorderStyle.FixedSingle;
            _expandedHeight = height;

            // ── Title bar (blue strip at top) ─────────────────────────
            _titleBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 32,
                BackColor = Color.FromArgb(70, 130, 180) // steel blue
            };

            // ── Title text ────────────────────────────────────────────
            _titleLabel = new Label
            {
                Text = title,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Location = new Point(8, 0),
                Size = new Size(width - 80, 32),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // ── Close button (×) ──────────────────────────────────────
            _btnClose = new Button
            {
                Text = "✕",
                Size = new Size(28, 28),
                Location = new Point(width - 34, 2),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 8f),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            _btnClose.FlatAppearance.BorderSize = 0;
            _btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(196, 43, 28);
            _btnClose.Click += (s, e) => this.Visible = false; // hide the panel

            // ── Minimize button (−) ───────────────────────────────────
            _btnMinimize = new Button
            {
                Text = "−",
                Size = new Size(28, 28),
                Location = new Point(width - 64, 2),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("Segoe UI", 11f),
                Cursor = Cursors.Hand,
                TabStop = false
            };
            _btnMinimize.FlatAppearance.BorderSize = 0;
            _btnMinimize.FlatAppearance.MouseOverBackColor = Color.FromArgb(50, 255, 255, 255);
            _btnMinimize.Click += ToggleMinimize;

            // ── Content area (where you put your controls) ────────────
            _contentArea = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(8)
            };

            // ── Drag to move ──────────────────────────────────────────
            _titleBar.MouseDown += (s, e) => { _dragging = true; _dragStart = e.Location; };
            _titleBar.MouseUp += (s, e) => { _dragging = false; };
            _titleBar.MouseMove += (s, e) =>
            {
                if (!_dragging) return;
                this.Left += e.X - _dragStart.X;
                this.Top += e.Y - _dragStart.Y;
            };
            _titleLabel.MouseDown += (s, e) => { _dragging = true; _dragStart = e.Location; };
            _titleLabel.MouseUp += (s, e) => { _dragging = false; };
            _titleLabel.MouseMove += (s, e) =>
            {
                if (!_dragging) return;
                this.Left += e.X - _dragStart.X;
                this.Top += e.Y - _dragStart.Y;
            };

            // ── Assemble ──────────────────────────────────────────────
            _titleBar.Controls.Add(_titleLabel);
            _titleBar.Controls.Add(_btnMinimize);
            _titleBar.Controls.Add(_btnClose);

            this.Controls.Add(_contentArea);
            this.Controls.Add(_titleBar);    // Dock=Top must be added last
        }

        // Collapse to just the title bar when − is clicked
        private void ToggleMinimize(object sender, EventArgs e)
        {
            _isMinimized = !_isMinimized;
            if (_isMinimized)
            {
                _contentArea.Visible = false;
                this.Height = _titleBar.Height + 2;
                _btnMinimize.Text = "□"; // restore icon
            }
            else
            {
                _contentArea.Visible = true;
                this.Height = _expandedHeight;
                _btnMinimize.Text = "−";
            }
        }
    }
}
