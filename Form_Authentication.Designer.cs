namespace Moto_List
{
    partial class Form_Authentication
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

      #region Windows Form Designer generated code

      /// <summary>
      ///  Required method for Designer support - do not modify
      ///  the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form_Authentication));
         label_Username = new Label();
         textBox_Username = new TextBox();
         label_Password = new Label();
         textBox_Password = new TextBox();
         button_login = new Button();
         button_Exit = new Button();
         SuspendLayout();
         // 
         // label_Username
         // 
         label_Username.AutoSize = true;
         label_Username.Location = new Point(278, 132);
         label_Username.Name = "label_Username";
         label_Username.Size = new Size(95, 25);
         label_Username.TabIndex = 0;
         label_Username.Text = "Username:";
         // 
         // textBox_Username
         // 
         textBox_Username.Location = new Point(379, 132);
         textBox_Username.Name = "textBox_Username";
         textBox_Username.Size = new Size(182, 31);
         textBox_Username.TabIndex = 1;
         // 
         // label_Password
         // 
         label_Password.AutoSize = true;
         label_Password.Location = new Point(282, 180);
         label_Password.Name = "label_Password";
         label_Password.Size = new Size(91, 25);
         label_Password.TabIndex = 2;
         label_Password.Text = "Password:";
         // 
         // textBox_Password
         // 
         textBox_Password.Location = new Point(379, 177);
         textBox_Password.Name = "textBox_Password";
         textBox_Password.Size = new Size(182, 31);
         textBox_Password.TabIndex = 3;
         // 
         // button_login
         // 
         button_login.Location = new Point(470, 214);
         button_login.Name = "button_login";
         button_login.Size = new Size(91, 35);
         button_login.TabIndex = 4;
         button_login.Text = "Login";
         button_login.UseVisualStyleBackColor = true;
         // 
         // button_Exit
         // 
         button_Exit.Location = new Point(373, 214);
         button_Exit.Name = "button_Exit";
         button_Exit.Size = new Size(91, 35);
         button_Exit.TabIndex = 5;
         button_Exit.Text = "Exit";
         button_Exit.UseVisualStyleBackColor = true;
         // 
         // Form_Authentication
         // 
         AutoScaleDimensions = new SizeF(10F, 25F);
         AutoScaleMode = AutoScaleMode.Font;
         BackgroundImage = (Image)resources.GetObject("$this.BackgroundImage");
         ClientSize = new Size(628, 590);
         Controls.Add(button_Exit);
         Controls.Add(button_login);
         Controls.Add(textBox_Password);
         Controls.Add(label_Password);
         Controls.Add(textBox_Username);
         Controls.Add(label_Username);
         Name = "Form_Authentication";
         Text = "Authentication";
         ResumeLayout(false);
         PerformLayout();
      }

      #endregion

      private Label label_Username;
      private TextBox textBox_Username;
      private Label label_Password;
      private TextBox textBox_Password;
      private Button button_login;
      private Button button_Exit;
   }
}
