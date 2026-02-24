# utils/controls_panel.py
# Cozy left-side controls panel for Paradisic Fields
# Built using a single shared braincell by Yours Truly and Grok (February 2026)

from PySide6.QtWidgets import (
    QWidget, QVBoxLayout, QHBoxLayout, QLabel, QPushButton
)
from PySide6.QtCore import Qt


def create_controls_panel(
    parent,
    on_load_callback,
    on_clear_callback,
    on_exit_callback,
    on_save_callback,
    on_load_session_callback,
    on_new_note_callback,
    on_delete_note_callback
):
    """
    Creates and returns the left controls panel widget.
    """
    panel = QWidget()
    layout = QVBoxLayout(panel)
    layout.setContentsMargins(20, 20, 20, 20)
    layout.setSpacing(16)

    # Friendly title
    title_label = QLabel("Paradisic Fields")
    title_label.setAlignment(Qt.AlignCenter)
    title_label.setStyleSheet("font-size: 18px; font-weight: bold; color: #6b5a4a;")
    layout.addWidget(title_label)

    # First button row: Load + Clear + Exit
    button_row = QHBoxLayout()
    button_row.setSpacing(12)

    load_button = QPushButton("Load Image ✨")
    load_button.setObjectName("loadFabricButton")
    load_button.setFixedHeight(50)
    load_button.clicked.connect(on_load_callback)
    button_row.addWidget(load_button)

    clear_button = QPushButton("Clear")
    clear_button.setObjectName("clearPreviewButton")
    clear_button.setFixedHeight(40)
    clear_button.setEnabled(False)
    clear_button.clicked.connect(on_clear_callback)
    button_row.addWidget(clear_button)

    exit_button = QPushButton("Exit")
    exit_button.setObjectName("exitButton")
    exit_button.setFixedHeight(40)
    exit_button.clicked.connect(on_exit_callback)
    button_row.addWidget(exit_button)

    layout.addLayout(button_row)

    # Second button row: New Note + Delete + Save/Load Session
    session_row = QHBoxLayout()
    session_row.setSpacing(12)

    new_note_button = QPushButton("New Note 📝")
    new_note_button.setObjectName("newNoteButton")
    new_note_button.setFixedHeight(40)
    new_note_button.clicked.connect(on_new_note_callback)
    session_row.addWidget(new_note_button)

    delete_button = QPushButton("Delete Note 🗑️")
    delete_button.setObjectName("deleteNoteButton")
    delete_button.setFixedHeight(40)
    delete_button.setEnabled(False)
    delete_button.clicked.connect(on_delete_note_callback)
    session_row.addWidget(delete_button)

    save_button = QPushButton("Save Session 💾")
    save_button.setObjectName("saveSessionButton")
    save_button.setFixedHeight(40)
    save_button.clicked.connect(on_save_callback)
    session_row.addWidget(save_button)

    load_session_button = QPushButton("Load Session 📂")
    load_session_button.setObjectName("loadSessionButton")
    load_session_button.setFixedHeight(40)
    load_session_button.clicked.connect(on_load_session_callback)
    session_row.addWidget(load_session_button)

    layout.addLayout(session_row)

    # Status / result area
    status_label = QLabel("Ready to enjoy! 🌟")
    status_label.setAlignment(Qt.AlignCenter)
    status_label.setWordWrap(True)
    status_label.setStyleSheet("font-size: 15px; color: #6b5a4a; padding: 10px;")
    layout.addWidget(status_label)
    layout.addStretch()

    return panel, load_button, clear_button, exit_button, save_button, load_session_button, new_note_button, delete_button, status_label