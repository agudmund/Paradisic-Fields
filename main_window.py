#!/usr/bin/env python3
# -*- coding: utf-8 -*-
#
# Paradisic Fields - main_window.py
# The cozy main UI window for enjoying
# Built using a single shared braincell by Yours Truly and Grok

from PySide6.QtWidgets import (
    QMainWindow,
    QApplication,
    QFileDialog,
    QWidget,
    QVBoxLayout,
    QSplitter,
    QLabel
)
from PySide6.QtCore import Qt, QThread, QPointF
from PySide6.QtGui import QIcon, QPixmap
from utils.file_dialog import open_image_dialog
from utils.control_panel import create_controls_panel
import logging
import cozy as Cozy

class Paradisic(QMainWindow):
    def __init__(self, logger=None):
        super().__init__()
        self.name = "paradisic_fields"
        self.icon = "images/appicon.png"
        self.display_name = self.name.replace("_", " ").title()

        self.logger = logger or logging.getLogger(self.name)
        self.setWindowIcon(QIcon(self.icon))
        self.setWindowTitle(self.display_name)
        self.resize(1480, 860)

        # Thread safety
        self.thread = None
        self.worker = None

        # Central splitter: Left column (controls + preview) | Right canvas
        self.main_splitter = QSplitter(Qt.Horizontal)
        self.setCentralWidget(self.main_splitter)

        # === LEFT COLUMN: Controls + Image Preview ===
        left_column = QWidget()
        left_layout = QVBoxLayout(left_column)
        left_layout.setContentsMargins(20, 20, 20, 20)
        left_layout.setSpacing(20)

        controls_panel, self.load_button, self.clear_button, self.exit_button, \
        self.save_button, self.load_session_button, self.new_note_button, \
        self.delete_button, self.status_label = create_controls_panel(
            self,
            on_load_callback=self._handle_load_button,
            on_clear_callback=self._clear_preview,
            on_exit_callback=self._handle_exit,
            on_save_callback=self._handle_save_session,
            on_load_session_callback=self._handle_load_session,
            on_new_note_callback=self._handle_new_note,
            on_delete_note_callback=self._handle_delete_selected
        )
        left_layout.addWidget(controls_panel)

        self.image_preview = QLabel()
        self.image_preview.setObjectName("imagePreview")
        self.image_preview.setAlignment(Qt.AlignCenter)
        self.image_preview.setScaledContents(True)
        self.image_preview.setMinimumHeight(420)
        self.image_preview.setText("Single image preview\n(Load to analyze)")
        self.image_preview.setStyleSheet(
            self.image_preview.styleSheet() + "color: #8a7a67; font-style: italic;"
        )
        left_layout.addWidget(self.image_preview, stretch=1)

        # === RIGHT: Full interactive canvas from global Cozy ===
        right_panel, self.canvas_view, self.canvas_scene = Cozy.Canvas.create_canvas_panel(self)

        self.main_splitter.addWidget(left_column)
        self.main_splitter.addWidget(right_panel)
        self.main_splitter.setSizes([520, 960])
        self.main_splitter.setCollapsible(0, True)

    def _handle_save_session(self):
        """Save the current sketchbook session — editable JSON + thumbnails 🌱"""
        filepath, _ = QFileDialog.getSaveFileName(
            self, "Save Sketchbook Session", "", "JSON Files (*.json)"
        )
        if filepath:
            Cozy.Session.save_session(self.canvas_scene, filepath)
            self.status_label.setText("Sketchbook session saved with love! 🌟")

    def _handle_load_session(self):
        """Load a saved sketchbook session — welcome back to your beautiful work!"""
        filepath, _ = QFileDialog.getOpenFileName(
            self, "Load Sketchbook Session", "", "JSON Files (*.json)"
        )
        if filepath:
            Cozy.Session.load_session(self.canvas_scene, filepath)
            self.status_label.setText("Sketchbook session loaded and ready to enjoy! 🌱")

    def _handle_new_note(self):
        """Creates a fresh, editable note on the canvas — instant inspiration! 📝"""
        # Find the next available node_id
        existing_ids = [item.node_id for item in self.canvas_scene.items() 
                       if isinstance(item, Cozy.WarmNode)]
        new_id = max(existing_ids, default=0) + 1

        # Gentle staggered position so notes don't pile up
        new_pos = QPointF(80 + (new_id % 5) * 60, 80 + (new_id // 5) * 80)

        new_node = Cozy.WarmNode(
            node_id=new_id,
            full_text="New note 🌱\n\n",
            pos=new_pos
        )
        self.canvas_scene.addItem(new_node)
        self.canvas_view.centerOn(new_node)

        self.status_label.setText(f"New note {new_id} created — double-click to edit! 📝")
        self.logger.info(f"New note {new_id} added to canvas")

    def _handle_delete_selected(self):
        """Delete any selected notes — works from button or keyboard."""
        selected = self.canvas_scene.selectedItems()
        deleted = 0
        for item in selected:
            if isinstance(item, Cozy.WarmNode):
                self.canvas_scene.removeItem(item)
                deleted += 1
        if deleted > 0:
            self.status_label.setText(f"Deleted {deleted} note{'s' if deleted > 1 else ''} 🗑️")
            self.logger.info(f"Deleted {deleted} selected note(s)")

            # Force immediate redraw so the canvas instantly shows the blank space
            self.canvas_scene.update()
            self.canvas_view.viewport().repaint()
                        
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
        self.worker = Cozy.UploadWorker(file_path)          # ← clean global style
        self.worker.moveToThread(self.thread)

        self.worker.status_updated.connect(self.status_label.setText)
        self.worker.finished.connect(self._on_analysis_finished)
        self.worker.error_occurred.connect(lambda msg: self.status_label.setText(f"Oops — {msg}"))

        self.thread.started.connect(self.worker.run)
        self.thread.finished.connect(self.thread.deleteLater)
        self.thread.start()

        self.clear_button.setEnabled(True)
        self.exit_button.setEnabled(True)

        if self.canvas_scene:
            selected_items = self.canvas_scene.selectedItems()
            if selected_items:
                for item in selected_items:
                    if isinstance(item, Cozy.WarmNode):     # ← clean global style
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

    def _on_analysis_finished(self, node_id, message):
        """Called when the background worker finishes — so gentle and celebratory!"""
        self.status_label.setText(message)
        self.logger.info(f"Background analysis complete for node {node_id}: {message}")

    def _clear_preview(self):
        self.image_preview.clear()
        self.image_preview.setText("Single image preview\n(Load to analyze)")
        self.status_label.setText("Ready to enjoy! 🌟")
        self.clear_button.setEnabled(False)
        self.logger.debug("Preview and status cleared")

    def _handle_exit(self):
        """Gentle exit — always clean and graceful 🌿"""
        self.logger.info("User chose to exit — closing Paradisic Fields with love")
        if self.thread is not None and self.thread.isRunning():
            self.thread.quit()
            if not self.thread.wait(800):
                self.thread.terminate()
                self.thread.wait()
            self.thread = None
            self.worker = None
        QApplication.instance().quit()


if __name__ == "__main__":
    import sys
    app = QApplication(sys.argv)
    app.setStyle("Fusion")
    window = Paradisic()
    window.show()
    sys.exit(app.exec())