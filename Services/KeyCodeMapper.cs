using System;
using WindowsInput.Native;

public class KeyCodeMapper
{
    public static VirtualKeyCode MapStringToVirtualKeyCode(string key)
    {
        switch (key.ToUpper()) 
        {
            // Alphabet keys
            case "A": return VirtualKeyCode.VK_A;
            case "B": return VirtualKeyCode.VK_B;
            case "C": return VirtualKeyCode.VK_C;
            case "D": return VirtualKeyCode.VK_D;
            case "E": return VirtualKeyCode.VK_E;
            case "F": return VirtualKeyCode.VK_F;
            case "G": return VirtualKeyCode.VK_G;
            case "H": return VirtualKeyCode.VK_H;
            case "I": return VirtualKeyCode.VK_I;
            case "J": return VirtualKeyCode.VK_J;
            case "K": return VirtualKeyCode.VK_K;
            case "L": return VirtualKeyCode.VK_L;
            case "M": return VirtualKeyCode.VK_M;
            case "N": return VirtualKeyCode.VK_N;
            case "O": return VirtualKeyCode.VK_O;
            case "P": return VirtualKeyCode.VK_P;
            case "Q": return VirtualKeyCode.VK_Q;
            case "R": return VirtualKeyCode.VK_R;
            case "S": return VirtualKeyCode.VK_S;
            case "T": return VirtualKeyCode.VK_T;
            case "U": return VirtualKeyCode.VK_U;
            case "V": return VirtualKeyCode.VK_V;
            case "W": return VirtualKeyCode.VK_W;
            case "X": return VirtualKeyCode.VK_X;
            case "Y": return VirtualKeyCode.VK_Y;
            case "Z": return VirtualKeyCode.VK_Z;

            // Number keys
            case "0": return VirtualKeyCode.VK_0;
            case "1": return VirtualKeyCode.VK_1;
            case "2": return VirtualKeyCode.VK_2;
            case "3": return VirtualKeyCode.VK_3;
            case "4": return VirtualKeyCode.VK_4;
            case "5": return VirtualKeyCode.VK_5;
            case "6": return VirtualKeyCode.VK_6;
            case "7": return VirtualKeyCode.VK_7;
            case "8": return VirtualKeyCode.VK_8;
            case "9": return VirtualKeyCode.VK_9;

            // Function keys
            case "F1": return VirtualKeyCode.F1;
            case "F2": return VirtualKeyCode.F2;
            case "F3": return VirtualKeyCode.F3;
            case "F4": return VirtualKeyCode.F4;
            case "F5": return VirtualKeyCode.F5;
            case "F6": return VirtualKeyCode.F6;
            case "F7": return VirtualKeyCode.F7;
            case "F8": return VirtualKeyCode.F8;
            case "F9": return VirtualKeyCode.F9;
            case "F10": return VirtualKeyCode.F10;
            case "F11": return VirtualKeyCode.F11;
            case "F12": return VirtualKeyCode.F12;

            // Modifier keys
            case "CTRL": return VirtualKeyCode.CONTROL;
            case "LCTRL": return VirtualKeyCode.LCONTROL;
            case "RCTRL": return VirtualKeyCode.RCONTROL;
            case "SHIFT": return VirtualKeyCode.SHIFT;
            case "LSHIFT": return VirtualKeyCode.LSHIFT;
            case "RSHIFT": return VirtualKeyCode.RSHIFT;
            case "ALT": return VirtualKeyCode.MENU;
            case "LALT": return VirtualKeyCode.LMENU;
            case "RALT": return VirtualKeyCode.RMENU;

            // Navigation keys
            case "LEFT": return VirtualKeyCode.LEFT;
            case "UP": return VirtualKeyCode.UP;
            case "RIGHT": return VirtualKeyCode.RIGHT;
            case "DOWN": return VirtualKeyCode.DOWN;
            case "HOME": return VirtualKeyCode.HOME;
            case "END": return VirtualKeyCode.END;
            case "PAGEUP": return VirtualKeyCode.PRIOR;
            case "PAGEDOWN": return VirtualKeyCode.NEXT;
            case "INSERT": return VirtualKeyCode.INSERT;
            case "DELETE": return VirtualKeyCode.DELETE;
            case "CAPITAL": return VirtualKeyCode.CAPITAL;

            // Special keys
            case "SPACE": return VirtualKeyCode.SPACE;
            case "ENTER": return VirtualKeyCode.RETURN;
            case "ESCAPE": return VirtualKeyCode.ESCAPE;
            case "MENU": return VirtualKeyCode.TAB;
            case "BACKSPACE": return VirtualKeyCode.BACK;
            case "PAUSE": return VirtualKeyCode.PAUSE;

            // Media keys
            case "MUTE": return VirtualKeyCode.VOLUME_MUTE;
            case "VOLUMEDOWN": return VirtualKeyCode.VOLUME_DOWN;
            case "VOLUMEUP": return VirtualKeyCode.VOLUME_UP;
            case "NEXTTRACK": return VirtualKeyCode.MEDIA_NEXT_TRACK;
            case "PREVTRACK": return VirtualKeyCode.MEDIA_PREV_TRACK;
            case "STOP": return VirtualKeyCode.MEDIA_STOP;
            case "PLAYPAUSE": return VirtualKeyCode.MEDIA_PLAY_PAUSE;

            // OEM keys
            case ";": return VirtualKeyCode.OEM_1;  // ;:
            case "=": return VirtualKeyCode.OEM_PLUS;  // + =
            case ",": return VirtualKeyCode.OEM_COMMA; // , <
            case "-": return VirtualKeyCode.OEM_MINUS; // - _
            case ".": return VirtualKeyCode.OEM_PERIOD; // . >
            case "/": return VirtualKeyCode.OEM_2;  // /?
            case "`": return VirtualKeyCode.OEM_3;  // `~
            case "[": return VirtualKeyCode.OEM_4;  // [{
            case "\\": return VirtualKeyCode.OEM_5;  // \|
            case "]": return VirtualKeyCode.OEM_6;  // ]}
            case "'": return VirtualKeyCode.OEM_7;  // '"

            default: throw new ArgumentException("Unsupported key");
        }
    }
}
