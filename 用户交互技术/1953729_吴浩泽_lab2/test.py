
import speech_recognition as sr

# Working with audio files
import win32api

r = sr.Recognizer()
speech = sr.AudioFile('f1lcapae.wav')
with speech as source:
    audio = r.record(source)
print(r.recognize_sphinx(audio))
win32api.ShellExecute(0, 'open', 'notepad.exe', '', '', 1)
'''
# Working with Microphones
mic = sr.Microphone()
with mic as source:
    r.adjust_for_ambient_noise(source)
    audio = r.listen(source)
r.recognize_sphinx(audio)
'''
