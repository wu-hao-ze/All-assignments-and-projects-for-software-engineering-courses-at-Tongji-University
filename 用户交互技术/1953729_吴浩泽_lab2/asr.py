import threading
import win32api
import webbrowser
from PyQt5 import QtWidgets, QtGui, QtCore, uic

from asrInterface import Ui_MainWindow
import sys

import speech_recognition as sr


class myWindow(QtWidgets.QMainWindow):

    def __init__(self):
        super(myWindow, self).__init__()
        self.myCommand = " "
        self.ui = Ui_MainWindow()
        self.ui.setupUi(self)
        self.myThread = None

    def showEvent(self, e):
        print("窗口显示")
        try:
            self.myThread = MyThread()
            self.myThread.start()
        except:
            print("thread start failed")

    def closeEvent(self, e):
        self.myThread.stop()
        print("closed")


class MyThread(threading.Thread):
    def __init__(self):
        super(MyThread, self).__init__()
        self._running = True

    def recognize_speech_from_mic(self, recognizer, microphone):
        """Transcribe speech from recorded from `microphone`.

        Returns a dictionary with three keys:
        "success": a boolean indicating whether or not the API request was
                   successful
        "error":   `None` if no error occured, otherwise a string containing
                   an error message if the API could not be reached or
                   speech was unrecognizable
        "transcription": `None` if speech could not be transcribed,
                   otherwise a string containing the transcribed text
        """
        # check that recognizer and microphone arguments are appropriate type
        if not isinstance(recognizer, sr.Recognizer):
            raise TypeError("`recognizer` must be `Recognizer` instance")

        if not isinstance(microphone, sr.Microphone):
            raise TypeError("`microphone` must be `Microphone` instance")

        # adjust the recognizer sensitivity to ambient noise and record audio
        # from the microphone
        with microphone as source:
            recognizer.adjust_for_ambient_noise(source)
            audio = recognizer.listen(source)

        # set up the response object
        response = {
            "success": True,
            "error": None,
            "transcription": None
        }

        # try recognizing the speech in the recording
        # if a RequestError or UnknownValueError exception is caught,
        #     update the response object accordingly
        try:
            response["transcription"] = recognizer.recognize_sphinx(audio)
        except sr.RequestError:
            # API was unreachable or unresponsive
            response["success"] = False
            response["error"] = "API unavailable"
        except sr.UnknownValueError:
            # speech was unintelligible
            response["error"] = "Unable to recognize speech"

        return response

    def stop(self):
        self._running = False
        print(self._running)

    def run(self):
        recognizer = sr.Recognizer()
        microphone = sr.Microphone()
        while self._running:
            res = self.recognize_speech_from_mic(recognizer, microphone)
            if res["error"]:
                print("ERROR: {}".format(res["error"]))
                continue
            words = res["transcription"]
            if self._running:
                print(words)
                if words.lower() in ["music", "you think"]:
                    win32api.ShellExecute(0, 'open', 'music.mp3', '', '', 1)
                elif words.lower() in ["notepad", "note pad", "goat's head", "goat head"]:
                    win32api.ShellExecute(0, 'open', 'notepad.exe', '', '', 1)
                elif words.lower() in ["web page", "webpage", "web"]:
                    webbrowser.open("https://www.baidu.com")
            else:
                break


if __name__ == '__main__':
    app = QtWidgets.QApplication([])
    application = myWindow()
    application.show()
    sys.exit(app.exec())
