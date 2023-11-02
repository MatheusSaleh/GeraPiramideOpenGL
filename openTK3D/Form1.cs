using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace openTK3D
{
    public partial class OpenTK3D : Form
    {
        int lateral = 0;
        Vector3d dir = new Vector3d(0, -450, 120);        //direção da câmera
        Vector3d pos = new Vector3d(0, -550, 120);     //posiçãoo da câmera
        float camera_rotation = 0;                     //rotação no eixo Z

        int sides = 3;

        public OpenTK3D()
        {
            InitializeComponent();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            GL.Translate(0, 0, -5); 

            
            GL.Begin(PrimitiveType.Triangles);

            
            for (int i = 0; i < sides; i++)
            {
                double angle1 = 2 * Math.PI * i / sides;
                double angle2 = 2 * Math.PI * (i + 1) / sides;

                GL.Color3(System.Drawing.Color.Red);
                GL.Vertex3(0, 1, 0);
                GL.Color3(System.Drawing.Color.Green);
                GL.Vertex3(Math.Cos(angle1), -1, Math.Sin(angle1));
                GL.Color3(System.Drawing.Color.Blue);
                GL.Vertex3(Math.Cos(angle2), -1, Math.Sin(angle2));
            }

            GL.End();
            glControl1.SwapBuffers();
        }
        private void glControl1_Load(object sender, EventArgs e)
        {
            GL.ClearColor(Color.Black);         // definindo a cor de limpeza do fundo da tela
            GL.Enable(EnableCap.Light0);

            SetupViewport();                      //configura a janela de pintura
        }

        private void SetupViewport() //configura a janela de projeção 
        {
            int w = glControl1.Width; //largura da janela
            int h = glControl1.Height; //altura da janela

            Matrix4 projection = Matrix4.CreatePerspectiveFieldOfView(1.0f, w / (float)h, 0.1f, 2000.0f);


            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity(); //zera a matriz de projeção com a matriz identidade

            GL.LoadMatrix(ref projection);

            GL.Viewport(0, 0, w, h); // usa toda area de pintura da glcontrol
            lateral = w / 2;

        }


        private void calcula_direcao()
        {
            dir.X = pos.X + (Math.Sin(camera_rotation * Math.PI / 180) * 100);
            dir.Y = pos.Y + (Math.Cos(camera_rotation * Math.PI / 180) * 100);
        }
        private void glControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > lateral)
            {
                camera_rotation += 2;
            }
            if (e.X < lateral)
            {
                camera_rotation -= 2;
            }
            lateral = e.X;
            calcula_direcao();
            glControl1.Invalidate();
        }

        private void glControl1_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            float a = camera_rotation;
            int tipoTecla = 0;
            if (e.KeyCode == Keys.Left)
            {
                a -= 90;
                tipoTecla = 1;
            }
            if (e.KeyCode == Keys.Right)
            {
                a += 90;
                tipoTecla = 1;
            }
            if (e.KeyCode == Keys.Up)
            { tipoTecla = 1; }
            if (e.KeyCode == Keys.Down)
            {
                a += 180;
                tipoTecla = 1;
            }

            if (e.KeyCode == Keys.D)
            {
                a += 1;
                tipoTecla = 2;
            }
            if (e.KeyCode == Keys.A)
            {
                a -= 1;
                tipoTecla = 2;
            }
            if (tipoTecla == 1)
            {
                if (a < 0) a += 360;
                if (a > 360) a -= 360;
                pos.X += (Math.Sin(a * Math.PI / 180) * 10);
                pos.Y += (Math.Cos(a * Math.PI / 180) * 10);
                calcula_direcao();
                glControl1.Invalidate();
            }

            if (tipoTecla == 2)
            {
                camera_rotation = a;
                calcula_direcao();
                glControl1.Invalidate();
            }
        }

        static int LoadTexture(string filename)
        {
            if (String.IsNullOrEmpty(filename))
                throw new ArgumentException(filename);

            int id;//= GL.GenTexture(); 

            GL.GenTextures(1, out id);
            GL.BindTexture(TextureTarget.Texture2D, id);

            Bitmap bmp = new Bitmap(filename);

            BitmapData data = bmp.LockBits(new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.Repeat);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.Repeat);

            return id;
        }



        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            sides = int.Parse(comboBox1.SelectedItem.ToString());
            glControl1.Invalidate();
        }
    }
}
