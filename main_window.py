#!/usr/bin/env python3
# -*- coding: utf-8 -*-
#
# Paradisic Fields - main_window.py
# The cozy main UI window for enjoying
# Built using a single shared braincell by Yours Truly and Grok

from PySide6.QtWidgets import (
    QMainWindow,
    QApplication,
    QWidget,
    QVBoxLayout,
    QSplitter,
    QLabel
)
from PySide6.QtCore import Qt, QThread
from PySide6.QtGui import QIcon, QPixmap
from utils.file_dialog import open_image_dialog
from utils.control_panel import create_controls_panel
from utils.WarmNode import WarmNode
import logging

import cozy as Cozy
from cozy import worker


class Paradisic(QMainWindow):
    def __init__(self, logger=None):
        super().__init__()
        self.name = "paradisic_fields"
        self.icon = "images/appicon.png"

        self.display_name = self.name.replace("_", " ").title()
        self.logger = logger or logging.getLogger(self.name)

        self.setWindowIcon(QIcon(self.icon))
        self.setWindowTitle(self.display_name)   # ← now dynamic and beautiful!
        self.resize(1480, 860)

        # Central splitter: Left column (controls + preview) | Right canvas
        self.main_splitter = QSplitter(Qt.Horizontal)
        self.setCentralWidget(self.main_splitter)

        # === LEFT COLUMN: Controls + Image Preview parked right underneath ===
        left_column = QWidget()
        left_layout = QVBoxLayout(left_column)
        left_layout.setContentsMargins(20, 20, 20, 20)
        left_layout.setSpacing(20)

        # Controls (title, Load/Clear buttons, status)
        controls_panel, self.load_button, self.clear_button, self.status_label = create_controls_panel(
            self,
            on_load_callback=self._handle_load_button,
            on_clear_callback=self._clear_preview
        )
        left_layout.addWidget(controls_panel)

        # Image Preview — now cozy and parked directly under the buttons!
        self.image_preview = QLabel()
        self.image_preview.setObjectName("imagePreview")
        self.image_preview.setAlignment(Qt.AlignCenter)
        self.image_preview.setScaledContents(True)
        self.image_preview.setMinimumHeight(420)  # lovely breathing room
        self.image_preview.setText("Single image preview\n(Load to analyze)")
        self.image_preview.setStyleSheet(
            self.image_preview.styleSheet() + "color: #8a7a67; font-style: italic;"
        )
        left_layout.addWidget(self.image_preview, stretch=1)

        # === RIGHT: Full interactive canvas ===
        right_panel, self.canvas_view, self.canvas_scene = Cozy.Canvas.create_canvas_panel(self)

        # Add to splitter
        self.main_splitter.addWidget(left_column)
        self.main_splitter.addWidget(right_panel)

        # Beautiful proportions: left column feels focused, canvas gets plenty of space
        self.main_splitter.setSizes([520, 960])
        self.main_splitter.setCollapsible(0, True)

    def _handle_load_button(self):
        file_path, success = open_image_dialog(
            self, logger=self.logger, image_preview=self.image_preview
        )
        if not success:
            return
        self.status_label.setText("Preparing gentle background analysis... 🌱")
        QApplication.processEvents()

        # Start the cozy worker in its own thread
        self.thread = QThread()
        self.worker = worker.UploadWorker(file_path)
        self.worker.moveToThread(self.thread)

        # Connect our warm signals
        self.worker.status_updated.connect(self.status_label.setText)
        self.worker.finished.connect(self._on_analysis_finished)
        self.worker.error_occurred.connect(lambda msg: self.status_label.setText(f"Oops — {msg}"))

        self.thread.started.connect(self.worker.run)
        self.thread.finished.connect(self.thread.deleteLater)
        self.thread.start()

        self.clear_button.setEnabled(True)
        if self.canvas_scene:
            selected_items = self.canvas_scene.selectedItems()
            if selected_items:
                for item in selected_items:
                    if isinstance(item, WarmNode):
                        pix = QPixmap(file_path)
                        if not pix.isNull():
                            item.set_thumbnail(pix)
                            self.logger.info(f"Thumbnail added to node {item.node_id}")
                            self.status_label.setText("Thumbnail added to selected node 🌟")
                            break
            else:
                self.status_label.setText(
                    "Image loaded! Select a node on the canvas to add thumbnail 🌱"
                )

    def _clear_preview(self):
        self.image_preview.clear()
        self.image_preview.setText("Single image preview\n(Load to analyze)")
        self.status_label.setText("Ready to enjoy! 🌟")
        self.clear_button.setEnabled(False)
        self.logger.debug("Preview and status cleared")

    def _on_analysis_finished(self, node_id, message):
        """Called when the background worker finishes — so gentle and celebratory!"""
        self.status_label.setText(message)
        self.logger.info(f"Background analysis complete for node {node_id}: {message}")
        # Future: you can update the selected WarmNode text, add a note, etc.


if __name__ == "__main__":
    import sys
    app = QApplication(sys.argv)
    app.setStyle("Fusion")
    window = Paradisic()
    window.show()
    sys.exit(app.exec())