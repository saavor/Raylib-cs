/*******************************************************************************************
*
*   raylib [textures] example - Mouse painting
*
*   This example has been created using raylib 2.5 (www.raylib.com)
*   raylib is licensed under an unmodified zlib/libpng license (View raylib.h for details)
*
*   Example contributed by Chris Dill (@MysteriousSpace) and reviewed by Ramon Santamaria (@raysan5)
*
*   Copyright (c) 2019 Chris Dill (@MysteriousSpace) and Ramon Santamaria (@raysan5)
*
********************************************************************************************/

using System.Numerics;
using static Raylib_cs.Raylib;

namespace Examples.Textures;

public class MousePainting
{
    public static int Main()
    {
        // Initialization
        //--------------------------------------------------------------------------------------
        const int screenWidth = 800;
        const int screenHeight = 450;

        InitWindow(screenWidth, screenHeight, "raylib [textures] example - mouse painting");

        // Colours to choose from
        Color[] colors = new Color[] {
                Color.RayWhite,
                Color.Yellow,
                Color.Gold,
                Color.Orange,
                Color.Pink,
                Color.Red,
                Color.Maroon,
                Color.Green,
                Color.Lime,
                Color.DarkGreen,
                Color.SkyBlue,
                Color.Blue,
                Color.DarkBlue,
                Color.Purple,
                Color.Violet,
                Color.DarkPurple,
                Color.Beige,
                Color.Brown,
                Color.DarkBrown,
                Color.LightGray,
                Color.Gray,
                Color.DarkGray,
                Color.Black
            };

        // Define colorsRecs data (for every rectangle)
        Rectangle[] colorsRecs = new Rectangle[colors.Length];

        for (int i = 0; i < colorsRecs.Length; i++)
        {
            colorsRecs[i].X = 10 + 30 * i + 2 * i;
            colorsRecs[i].Y = 10;
            colorsRecs[i].Width = 30;
            colorsRecs[i].Height = 30;
        }

        int colorSelected = 0;
        int colorSelectedPrev = colorSelected;
        int colorMouseHover = 0;
        int brushSize = 20;

        Rectangle btnSaveRec = new(750, 10, 40, 30);
        bool btnSaveMouseHover = false;
        bool showSaveMessage = false;
        int saveMessageCounter = 0;

        // Create a RenderTexture2D to use as a canvas
        RenderTexture2D target = LoadRenderTexture(screenWidth, screenHeight);

        // Clear render texture before entering the game loop
        BeginTextureMode(target);
        ClearBackground(colors[0]);
        EndTextureMode();

        SetTargetFPS(120);              // Set our game to run at 120 frames-per-second
        //--------------------------------------------------------------------------------------

        // Main game loop
        while (!WindowShouldClose())
        {
            // Update
            //----------------------------------------------------------------------------------
            Vector2 mousePos = GetMousePosition();

            // Move between colors with keys
            if (IsKeyPressed(KeyboardKey.Right))
            {
                colorSelected++;
            }
            else if (IsKeyPressed(KeyboardKey.Left))
            {
                colorSelected--;
            }

            if (colorSelected >= colors.Length)
            {
                colorSelected = colors.Length - 1;
            }
            else if (colorSelected < 0)
            {
                colorSelected = 0;
            }

            // Choose color with mouse
            for (int i = 0; i < colors.Length; i++)
            {
                if (CheckCollisionPointRec(mousePos, colorsRecs[i]))
                {
                    colorMouseHover = i;
                    break;
                }
                else
                {
                    colorMouseHover = -1;
                }
            }

            if ((colorMouseHover >= 0) && IsMouseButtonPressed(MouseButton.Left))
            {
                colorSelected = colorMouseHover;
                colorSelectedPrev = colorSelected;
            }

            // Change brush size
            brushSize += (int)(GetMouseWheelMove() * 5);
            if (brushSize < 2)
            {
                brushSize = 2;
            }

            if (brushSize > 50)
            {
                brushSize = 50;
            }

            if (IsKeyPressed(KeyboardKey.C))
            {
                // Clear render texture to clear color
                BeginTextureMode(target);
                ClearBackground(colors[0]);
                EndTextureMode();
            }

            if (IsMouseButtonDown(MouseButton.Left))
            {
                // Paint circle into render texture
                // NOTE: To avoid discontinuous circles, we could store
                // previous-next mouse points and just draw a line using brush size
                BeginTextureMode(target);
                if (mousePos.Y > 50)
                {
                    DrawCircle((int)mousePos.X, (int)mousePos.Y, brushSize, colors[colorSelected]);
                }

                EndTextureMode();
            }
            else if (IsMouseButtonDown(MouseButton.Right))
            {
                colorSelected = 0;

                // Erase circle from render texture
                BeginTextureMode(target);
                if (mousePos.Y > 50)
                {
                    DrawCircle((int)mousePos.X, (int)mousePos.Y, brushSize, colors[0]);
                }

                EndTextureMode();
            }
            else
            {
                colorSelected = colorSelectedPrev;
            }

            // Check mouse hover save button
            if (CheckCollisionPointRec(mousePos, btnSaveRec))
            {
                btnSaveMouseHover = true;
            }
            else
            {
                btnSaveMouseHover = false;
            }

            // Image saving logic
            // NOTE: Saving painted texture to a default named image
            if ((btnSaveMouseHover && IsMouseButtonReleased(MouseButton.Left)) ||
                IsKeyPressed(KeyboardKey.S))
            {
                Image image = LoadImageFromTexture(target.Texture);
                ImageFlipVertical(ref image);
                ExportImage(image, "my_amazing_texture_painting.png");
                UnloadImage(image);
                showSaveMessage = true;
            }

            if (showSaveMessage)
            {
                // On saving, show a full screen message for 2 seconds
                saveMessageCounter++;
                if (saveMessageCounter > 240)
                {
                    showSaveMessage = false;
                    saveMessageCounter = 0;
                }
            }
            //----------------------------------------------------------------------------------

            // Draw
            //----------------------------------------------------------------------------------
            BeginDrawing();
            ClearBackground(Color.RayWhite);

            // NOTE: Render texture must be y-flipped due to default OpenGL coordinates (left-bottom)
            Rectangle source = new(0, 0, target.Texture.Width, -target.Texture.Height);
            DrawTextureRec(target.Texture, source, new Vector2(0, 0), Color.White);

            // Draw drawing circle for reference
            if (mousePos.Y > 50)
            {
                if (IsMouseButtonDown(MouseButton.Right))
                {
                    DrawCircleLines((int)mousePos.X, (int)mousePos.Y, brushSize, colors[colorSelected]);
                }
                else
                {
                    DrawCircle(GetMouseX(), GetMouseY(), brushSize, colors[colorSelected]);
                }
            }

            // Draw top panel
            DrawRectangle(0, 0, GetScreenWidth(), 50, Color.RayWhite);
            DrawLine(0, 50, GetScreenWidth(), 50, Color.LightGray);

            // Draw color selection rectangles
            for (int i = 0; i < colors.Length; i++)
            {
                DrawRectangleRec(colorsRecs[i], colors[i]);
            }

            DrawRectangleLines(10, 10, 30, 30, Color.LightGray);

            if (colorMouseHover >= 0)
            {
                DrawRectangleRec(colorsRecs[colorMouseHover], ColorAlpha(Color.White, 0.6f));
            }

            Rectangle rec = new(
                colorsRecs[colorSelected].X - 2,
                colorsRecs[colorSelected].Y - 2,
                colorsRecs[colorSelected].Width + 4,
                colorsRecs[colorSelected].Height + 4
            );
            DrawRectangleLinesEx(rec, 2, Color.Black);

            // Draw save image button
            DrawRectangleLinesEx(btnSaveRec, 2, btnSaveMouseHover ? Color.Red : Color.Black);
            DrawText("SAVE!", 755, 20, 10, btnSaveMouseHover ? Color.Red : Color.Black);

            // Draw save image message
            if (showSaveMessage)
            {
                DrawRectangle(0, 0, GetScreenWidth(), GetScreenHeight(), ColorAlpha(Color.RayWhite, 0.8f));
                DrawRectangle(0, 150, GetScreenWidth(), 80, Color.Black);
                DrawText("IMAGE SAVED:  my_amazing_texture_painting.png", 150, 180, 20, Color.RayWhite);
            }

            EndDrawing();
            //----------------------------------------------------------------------------------
        }

        // De-Initialization
        //--------------------------------------------------------------------------------------
        UnloadRenderTexture(target);

        CloseWindow();
        //--------------------------------------------------------------------------------------

        return 0;
    }
}
