import socket
import ctypes
import time
import asyncio
import threading
from enum import Enum

portToListen=2507


key_code_1= 0x31 # 1 //Fishing key
key_code_interaction= 0x46 # F //Interact with ocean
key_code_interaction= 0x09 # Tab // To test
key_code_space= 0x20 # Space // To test
fishing_mode = "Ocean"
fishing_mode = "Lane"


time_between_keyPress = 0.1


state_dictionary = {}



class WindowIdExecutor():
    def __init__(self, window_id):
        self.fishing_time =20
        self.recall_time = 7
        self.jump_time = 3
        self.mute_sound_time = 10
        self.window_id = window_id
        self.first_sound_received_time =get_current_time()
        self.time_recall_late = get_current_time()
        self.time_jump = get_current_time()
        self.time_missed_fishing = get_current_time()
        
        
    def received_sound(self):
        time = get_current_time()
        time_since_first_sound = time - self.first_sound_received_time
        if time_since_first_sound < self.mute_sound_time:
            return
        self.reset_timer()
    
    def reset_timer(self):
        time = get_current_time()
        self.first_sound_received_time = time
        self.time_recall_late = time
        self.time_jump =time + self.recall_time
        self.time_cast_fishing = time + self.recall_time + self.jump_time
        self.time_missed_fishing = time + self.recall_time + self.jump_time + self.fishing_time
    
    def is_time_jump(self, previous, current):
        if(  self.time_jump > previous and self.time_jump <= current):
            return True
        return False
    
    def is_time_cast_fishing(self, previous, current):
        if(  self.time_cast_fishing > previous and self.time_cast_fishing <= current):
            return True
        return False
    
    def is_time_recall_late(self, previous, current):
        if(  self.time_recall_late > previous and self.time_recall_late <= current):
            return True
        return False
   
        
    def is_waiting_for_to_long(self, current):
        if( current> self.time_missed_fishing ):
            return True
        return False
    
    



def execute_future_action(future_action, window_id):
    print(f"A:{future_action} W:{str(window_id)}")
    if future_action == "cast_fishing":
        cast_fishing(window_id)
    elif future_action== "recovert_line":
        recovert_line(window_id)
    elif future_action == "jump":
        jump(window_id)
    else:
        print("Unknown action")
    


def get_current_time():
    return time.time()


def send_key(window_id, key_code):
    print(f"Sending key code {key_code} to window {window_id}")
    ctypes.windll.user32.PostMessageW(window_id, 0x100, key_code, 0)
    time.sleep(time_between_keyPress)
    ctypes.windll.user32.PostMessageW(window_id, 0x101, key_code, 0)



def notify_fishing(window_id):
    for state in state_dictionary.values():
        if state.window_id == window_id:
            state.received_sound()
            return
    


def jump(window_id):
    print("jump")
    send_key(window_id, key_code_space)
    
def recovert_line(window_id):
    print("Recovering line")
    send_key(window_id, key_code_interaction)

def cast_fishing(window_id):
    print("Casting fishing")
    if fishing_mode == "Ocean":
        send_key(window_id, key_code_interaction)
    else:
        send_key(window_id, key_code_1)


def listen_udp(port):
    # Create a UDP socket
    sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)

    # Bind the socket to a specific address and port
    sock.bind(('0.0.0.0', port))

    print(f"       Listening for UDP packets on port {port}...")

    while True:
        # Receive data from the socket
        data, addr = sock.recvfrom(1024)
        decoded_data = data.decode('utf-8', 'ignore')
        decoded_data = decoded_data.replace(' ' ,' ')
        tokens = decoded_data.split(":")
        if tokens[0].strip() == "1":
            int_window_id = int(tokens[1])
            int_array_index = int(tokens[2])
            int_name = (tokens[3])
            if state_dictionary.get(int_window_id) is None:
                state_dictionary[int_window_id] = WindowIdExecutor(int_window_id)
                state_dictionary[int_window_id].reset_timer()
            state_dictionary[int_window_id].received_sound()
                
                
                
                
            


def tick_timer():
    print ("     Starting recast_fishing_if_cooldown_past")
    previous_time = get_current_time()
    current_time = get_current_time()
    while True:
        preivous_time = current_time
        current_time = get_current_time()
        if not( current_time == previous_time) :
            for state in state_dictionary.values():
                if state.is_time_jump(previous_time, current_time):
                    execute_future_action("jump", state.window_id)
                if state.is_time_cast_fishing(previous_time, current_time):
                    execute_future_action("cast_fishing", state.window_id)
                if state.is_waiting_for_to_long(current_time):
                    execute_future_action("recovert_line", state.window_id)
                    state.reset_timer()
        time.sleep(1)
        print(".")

 

        # Run recast_fishing_if_cooldown_past and listen_udp in parallel threads
if __name__ == "__main__":

    # Create and start the recast_fishing_if_cooldown_past thread
    recast_thread = threading.Thread(target=tick_timer)
    recast_thread.start()

    # Create and start the listen_udp thread
    listen_thread = threading.Thread(target=listen_udp, args=(portToListen,))
    listen_thread.start()

string_input = input("Press Enter to exit")




# State machine
# For each window_id
    # Waiting for sound to be received
    # Sound received, lock the the event
    # recall the lane fishing
    # wait for to be recalled
    # jump
    # wait end of jump
    # cast fishing
    # wait for fishing to be completed
    # no sound received, recall the lane fishing


