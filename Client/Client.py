#!/usr/bin/env python3
# Copyright 2017 Google Inc.
#
# Licensed under the Apache License, Version 2.0 (the "License");
# you may not use this file except in compliance with the License.
# You may obtain a copy of the License at
#
#     http://www.apache.org/licenses/LICENSE-2.0
#
# Unless required by applicable law or agreed to in writing, software
# distributed under the License is distributed on an "AS IS" BASIS,
# WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
# See the License for the specific language governing permissions and
# limitations under the License.

"""Check that the voiceHAT audio input and output are both working."""

import os
import sys
import tempfile
import textwrap
import traceback
import socket


sys.path.append(os.path.realpath(os.path.join(__file__, '..', '..')) + '/src/')


CARDS_PATH = '/proc/asound/cards'
VOICEHAT_ID = 'googlevoicehat'

STOP_DELAY = 1.0

TEST_SOUND_PATH = '/usr/share/sounds/alsa/Front_Center.wav'

RECORD_DURATION_SECONDS = 3

HOST = "172.16.0.174"
PORT = 2000




def check_speaker_works():
    """Check the speaker makes a sound."""
    print('Playing a test sound...')
    aiy.audio.play_wave(TEST_SOUND_PATH)




def check_mic_works():
    """Check the microphone records correctly."""
    temp_file, temp_path = tempfile.mkstemp(suffix='.wav')
    os.close(temp_file)

    try:
        input("When you're ready, press enter and say your phrase")
        print('Recording...')
        aiy.audio.record_to_wave(temp_path, RECORD_DURATION_SECONDS)
        print('Analysing')
        aiy.audio.play_wave(temp_path)
    finally:
        try:
            os.unlink(temp_path)
        except FileNotFoundError:
            pass

def AIY_Assistant():
    temp_file, temp_path = tempfile.mkstemp(suffix='.wav')
    os.close(temp_file)
    
    try:
        input("Press enter and say your phrase")
        print("Recording")
        aiy.audio.record_to_wave(temp_path, RECORD_DURATION_SECONDS)
        print('Analysing')

        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect((HOST,PORT))
        with open(temp_path,'rb') as f:
            for l in f: s.sendall(l)
            bytesToExpectString = s.recv(6).decode('utf-8')
            bytesToExpect = int (bytesToExpectString)

        return s.recv(bytesToExpect).decode('utf-8')

        
        
    finally:
        try:
            os.unlink(temp_path)
        except FileNotFoundError:
            pass


def Test_Token():
    try:
        print("Testing Token:")
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect((HOST,PORT))
        s.send(b"T")
        bytesBack = int(s.recv(6).decode('utf-8'))
        reply = s.recv(bytesBack).decode('utf-8')
        print("GOT: "+reply)
            
        
    except socket.error:
        print("Ping Error")
    

def Test_Ping():
    try:
        print("Testing ping:")
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect((HOST,PORT))
        s.send(b"P")
        bytesBack = int(s.recv(6).decode('utf-8'))
        reply = s.recv(bytesBack).decode('utf-8')
        if reply == "pong":
            print("Ping pong received and sent")
        else:
            print("Ping sent but no pong")
            
        
    except socket.error:
        print("Ping Error")


def Practice_File():
    try:
        temp_path = ""
        print('Analysing')
        s = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        s.connect((HOST,PORT))
        with open(temp_path,'rb') as f:
            for l in f: s.sendall(l)
            bytesToExpectString = s.recv(6).decode('utf-8')
            bytesToExpect = int (bytesToExpectString)

        return s.recv(bytesToExpect).decode('utf-8')

        
        
    finally:
        try:
            os.unlink(temp_path)
        except FileNotFoundError:
            pass



def main():
    Test_Ping()
    Test_Token()
    

    


if __name__ == '__main__':
        main()
        input('Press Enter to close...')
