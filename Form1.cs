﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
 using System.Data.SqlTypes;
 using System.Drawing;
using System.Linq;
 using System.Media;
 using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Dimitri_Brancourt_WAF
{
    public partial class Form1 : Form
    {

        bool goLeft, goRight, jumping, isGameOver, isGrounded, isRunning;
        private bool isLeft = true;

        int jumpSpeed;
        int force;
        int score = 0;
        int playerSpeed = 7;

        int horizontalSpeed = 5;
        int verticalSpeed = 5;

        int enemyOneSpeed = 5;
        private int enemyTwoSpeed = 3;
        
        private SoundPlayer music;
        
        

        public Form1()
        {
            InitializeComponent();
            music = new SoundPlayer(Properties.Resources.badApple);
        }

        // Main function which makes the start and end of game, and handles the collisions
        // Also allows to inplement the player movement
        private void MainGameTimerEvent(object sender, EventArgs e)
        {
            music.PlayLooping();
            // player movement, the verification of the keys are below this function
            txtScore.Text = "Score: " + score;
            player.Top += jumpSpeed;

            if (goLeft)
            {
                player.Left -= playerSpeed;
            }

            if (goRight)
            {
                player.Left += playerSpeed;
            }

            if (jumping && force < 0)
                jumping = false;

            if (jumping)
            {
                jumpSpeed = -9;
                force -= 1;
            }
            else
                jumpSpeed = 12;
            
            // Verification of collisions between player and platforms
            foreach (Control x in Controls)
            {
                if (x is PictureBox)
                {
                    if ((string)x.Tag == "platform")
                    {
                        if (player.Bounds.IntersectsWith(x.Bounds))
                        {
                            force = 9;
                            player.Top = x.Top - player.Height;

                            if (x.Name == "horizontalPlateform" && goLeft == false || x.Name == "horizontalPlateform" && goRight == false)
                            {
                                player.Left -= horizontalSpeed;
                            }
                        }
                    }
                }
            }

            // Verification of collisions with coins
            foreach (Control x in Controls)
            {
                if (x is PictureBox)
                {
                    if ((string)x.Tag == "coin")
                    {
                        if (player.Bounds.IntersectsWith(x.Bounds) && x.Visible)
                        {
                            x.Visible = false;
                            score++;
                        }
                    }
                }
            }

            // Verification of collisions with enemies
            foreach (Control x in Controls)
            {
                if (x is PictureBox)
                {
                    if ((string)x.Tag == "enemy")
                    {
                        if (player.Bounds.IntersectsWith(x.Bounds))
                        {
                            gameTimer.Stop();
                            isGameOver = true;
                            txtScore.Text = "Score: " + score + Environment.NewLine + "You were chomped !!";
                        }
                    }
                }

                x.BringToFront();
            }
            
            // Moving plateforms, with their speed and their max position
            horizontalPlatform.Left -= horizontalSpeed;
            if (horizontalPlatform.Left < 0 || horizontalPlatform.Left + horizontalPlatform.Width > ClientSize.Width)
                horizontalSpeed = -horizontalSpeed;
            
            verticalPlatform.Top += verticalSpeed;
            if (verticalPlatform.Top < 195 || verticalPlatform.Top > 581)
                verticalSpeed = -verticalSpeed;
            
            // Moving enemies, with their speed and their max position
            enemyOne.Left -= enemyOneSpeed;
            if (enemyOne.Left < pictureBox5.Left ||
                enemyOne.Left + enemyOne.Width > pictureBox5.Left + pictureBox5.Width)
            {
                enemyOne.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                enemyOneSpeed = -enemyOneSpeed;
            }
            
            enemyTwo.Left += enemyTwoSpeed;
            if (enemyTwo.Left < pictureBox2.Left ||
                enemyTwo.Left + enemyTwo.Width > pictureBox2.Left + pictureBox2.Width)
            {
                enemyTwo.Image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                enemyTwoSpeed = -enemyTwoSpeed;
            }
            
            if (player.Top + player.Height > ClientSize.Height + 50)
            {
                gameTimer.Stop();
                isGameOver = true;
                txtScore.Text = "Score: " + score + Environment.NewLine + "You fell from high place!";
            }

            if (player.Bounds.IntersectsWith(door.Bounds) && score >= 26)
            {
                gameTimer.Stop();
                isGameOver = true;
                txtScore.Text = "Score: " + score + Environment.NewLine + "Your quest is complete!";
            }
            else
            if (!isGameOver)
                    txtScore.Text = "Score: " + score + Environment.NewLine + "Collect all the coins";
            
        }
        
        // verifies if the key corresponding to the movement is maintained
        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    if (!isRunning)
                    {
                        player.Image = Properties.Resources.running;
                        isRunning = true;
                    }
                    
                    goLeft = true;
                    break;
                case Keys.D:
                    if (!isRunning)
                    {
                        player.Image = Properties.Resources.running2;
                        isRunning = true;
                    }
                    
                    goRight = true;
                    break;
                case Keys.Z when jumping == false:
                    jumping = true;
                    break;
            }
        }

        // verifies if the key is removed
        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Q:
                    goLeft = false;
                    isRunning = false;
                    player.Image = Properties.Resources.idle1;
                    break;
                case Keys.D:
                    goRight = false;
                    isRunning = false;
                    player.Image = Properties.Resources.idle1;
                    break;
            }

            if (jumping)
                jumping = false;
            
            if (e.KeyCode == Keys.Enter && isGameOver)
                RestartGame();
        }

        private void RestartGame()
        {

            jumping = false;
            goLeft = false;
            goRight = false;
            isGameOver = false;
            score = 0;

            txtScore.Text = "Score: " + score;

            foreach (Control x in Controls)
                if (x is PictureBox && x.Visible == false)
                    x.Visible = true;
            
            // reset the position of player, platform and enemies

            player.Left = 26;
            player.Top = 623;

            enemyOne.Left = 471;
            enemyTwo.Left = 360;

            horizontalPlatform.Left = 275;
            verticalPlatform.Top = 581;

            gameTimer.Start();
        }
    }
}