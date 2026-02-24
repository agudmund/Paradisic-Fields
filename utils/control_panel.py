# utils/controls_panel.py
# Cozy left-side controls panel for Paradisic Fields
# Built using a single shared braincell by Yours Truly and Grok (February 2026)

from PySide6.QtWidgets import (
    QWidget, QVBoxLayout, QHBoxLayout, QLabel, QPushButton
)
from PySide6.QtCore import Qt


def create_controls_panel(parent, on_load_callback, on_clear_callback):
    """
    Creates and returns the left controls panel widget.
    
    Args:
        parent: The parent widget (usually FabricIdentifier instance)
        on_load_callback: Function to call when Load button is clicked
        on_clear_callback: Function to call when Clear button is clicked
    
    Returns: QWidget (the fully built left panel)
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

    # Button row: Load + Clear
    button_row = QHBoxLayout()
    button_row.setSpacing(12)

    load_button = QPushButton("Load Image ✨")
    load_button.setObjectName("loadParadisicButton")
    load_button.setFixedHeight(50)
    load_button.clicked.connect(on_load_callback)
    button_row.addWidget(load_button)

    clear_button = QPushButton("Clear")
    clear_button.setObjectName("clearPreviewButton")
    clear_button.setFixedHeight(40)
    clear_button.setEnabled(False)
    clear_button.clicked.connect(on_clear_callback)
    button_row.addWidget(clear_button)

    layout.addLayout(button_row)

    # Status / result area
    status_label = QLabel("Ready to enjoy! 🌟")
    status_label.setAlignment(Qt.AlignCenter)
    status_label.setWordWrap(True)
    status_label.setStyleSheet("font-size: 15px; color: #6b5a4a; padding: 10px;")
    layout.addWidget(status_label)

    layout.addStretch()  # push content upward

    # Return the panel + references to buttons/label so main window can control them
    return panel, load_button, clear_button, status_label