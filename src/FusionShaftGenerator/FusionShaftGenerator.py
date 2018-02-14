#Author-Alex Vengrin
#Description-Shaft Component Generator for Autodesk Fusion 360

import adsk.core, adsk.fusion, adsk.cam, traceback
import tkinter as tk
import math


# Globals
app = []        # fusion application
ui = []         # fusion user interface
root = []       # tkinter app window
design = []
command_id = "ShaftGenerator"
workspace_to_use = "FusionSolidEnvironment"
panel_to_use = "SolidCreatePanel"
handlers = []



def command_definition_by_id(id):
    # <ui> variable was initialized in the <run> function
    if not id:
        ui.messageBox('command_definition id is not specified')
        return None
    command_definitions = ui.commandDefinitions
    command_definition = command_definitions.itemById(id)
    return command_definition


def command_control_by_id_for_panel(id):
    # <ui> variable was initialized in the <run> function
    if not id:
        ui.messageBox('commandControl id is not specified')
        return None
    workspaces = ui.workspaces
    modeling_workspace = workspaces.itemById(workspace_to_use)
    toolbar_panels = modeling_workspace.toolbarPanels
    toolbar_panel = toolbar_panels.itemById(panel_to_use)
    toolbar_controls = toolbar_panel.controls
    toolbar_control = toolbar_controls.itemById(id)
    return toolbar_control


def destroy_object(ui_obj, to_be_delete_obj):
    if ui_obj and to_be_delete_obj:
        if to_be_delete_obj.isValid:
            to_be_delete_obj.deleteMe()
        else:
            ui_obj.messageBox('to_be_delete_obj is not a valid object')
            

def initialize_tkinter_app():
    global root
    root = tk.Tk()
    #
    # Set tkinter window properties   
    #
    screen_width = float(root.winfo_screenwidth())
    screen_height = float(root.winfo_screenheight())
    pad_left = math.ceil(screen_width / 20)
    pad_top = math.ceil(screen_height / 3)
    root.title("Shaft Component Generator")
    root.geometry("400x300+{left}+{top}".format(left=pad_left, top=pad_top))
    root.resizable(width=tk.FALSE, height=tk.FALSE)
    
    #
    # Set window widgets
    #
    test_btn = tk.Button(master=root,
                         text="Test",
                         background="#555",
                         foreground="#ccc",
                         padx="20",
                         pady="8",
                         font="16",
                         command=on_test_btn_click)
    test_btn.pack()
    

def run(context):
    ui = None
    try:
        global app
        app = adsk.core.Application.get()
        global ui
        ui  = app.userInterface
        command_name = "Create Shaft Component"
        command_description = "Shaft Component Generator\nAMC Bridge - Education Project"
        command_resources = './resources/command'
        
        class CommandExecuteHandler(adsk.core.CommandEventHandler):
            def __init__(self):
                super().__init__()
            def notify(self, args):
                try:
                  initialize_tkinter_app()
                  root.mainloop() # start tkinter app
                except:
                    if ui:
                        ui.messageBox('command executed failed:\n{}'.format(traceback.format_exc()))
                        
        class CommandCreatedEventHandlerPanel(adsk.core.CommandCreatedEventHandler):
            def __init__(self):
                super().__init__() 
            def notify(self, args):
                try:
                    cmd = args.command
                    on_execute = CommandExecuteHandler()
                    cmd.execute.add(on_execute)
                    # keep the handler referenced beyond this function
                    handlers.append(on_execute)

                except:
                    if ui:
                        ui.messageBox('Panel command created failed:\n{}'.format(traceback.format_exc()))

        class CommandCreatedEventHandlerQAT(adsk.core.CommandCreatedEventHandler):
            def __init__(self):
                super().__init__()
            def notify(self, args):
                try:
                    command = args.command
                    on_execute = CommandExecuteHandler()
                    command.execute.add(on_execute)
                    # keep the handler referenced beyond this function
                    handlers.append(on_execute)

                except:
                    ui.messageBox('QAT command created failed:\n{}'.format(traceback.format_exc()))
                    
        command_definitions = ui.commandDefinitions
        
        # check if we have the command definition
        command_definition = command_definitions.itemById(command_id)
        if not command_definition:
            command_definition = command_definitions.addButtonDefinition(command_id,
                                                                         command_name,
                                                                         command_description,
                                                                         command_resources)
        on_command_created = CommandCreatedEventHandlerPanel()
        command_definition.commandCreated.add(on_command_created)
        # keep the handler referenced beyond this function
        handlers.append(on_command_created)
        
        # add a command on create panel in modeling workspace
        workspaces = ui.workspaces
        modeling_workspace = workspaces.itemById(workspace_to_use)
        toolbar_panels = modeling_workspace.toolbarPanels
        toolbar_panel = toolbar_panels.itemById(panel_to_use)
        toolbar_controls_panel = toolbar_panel.controls
        toolbar_control_panel = toolbar_controls_panel.itemById(command_id)
        if not toolbar_control_panel:
            toolbar_control_panel = toolbar_controls_panel.addCommand(command_definition, "")
            toolbar_control_panel.isVisible = True

    except:
        if ui:
            ui.messageBox('Failed:\n{}'.format(traceback.format_exc()))


def on_test_btn_click():
    ui.messageBox("Works!")

    
def stop(context):
    ui = None
    try:
        app = adsk.core.Application.get()
        ui  = app.userInterface
        obj_array = []
        
        command_control_panel = command_control_by_id_for_panel(command_id)
        if command_control_panel:
            obj_array.append(command_control_panel)
            
        command_definition = command_definition_by_id(command_id)
        if command_definition:
            obj_array.append(command_definition)
            
        for obj in obj_array:
            destroy_object(ui, obj)

    except:
        if ui:
            ui.messageBox('Add in stop FAILED:\n{}'.format(traceback.format_exc()))
