import {
  ChangeDetectorRef,
  Component,
  OnInit,
  inject
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  Decision
} from '../../../core/models/decision.model';

import {
  DecisionService
} from '../../../core/services/decision.service';

import {
  Router
} from '@angular/router';

import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-decision-list',
  standalone: true,
  imports: [
    CommonModule,
    DatePipe
  ],
  templateUrl: './decision-list.html',
  styleUrl: './decision-list.css'
})
export class DecisionListComponent
  implements OnInit {

  private readonly decisionService =
    inject(DecisionService);

  private readonly changeDetector =
    inject(ChangeDetectorRef);

  private readonly router =
    inject(Router);

  decisions: Decision[] = [];

  isLoading = false;

  errorMessage = '';

  decisionToDelete: Decision | null = null;

  isDeleting = false;

  deleteErrorMessage = '';

  ngOnInit(): void {
    this.loadDecisions();
  }

  loadDecisions(): void {
    this.isLoading = true;
    this.errorMessage = '';

    this.decisionService
      .getDecisions()
      .subscribe({

        next: (decisions) => {

          console.log(
            'Decisions received:',
            decisions
          );

          this.decisions = decisions;

          this.isLoading = false;

          this.changeDetector.markForCheck();
        },

        error: (error) => {

          console.error(
            'Failed to load decisions',
            error
          );

          this.errorMessage =
            'Unable to load decisions. Please try again.';

          this.isLoading = false;

          this.changeDetector.markForCheck();
        }

      });
  }

  createDecision(): void {
    this.router.navigate([
      '/decisions',
      'new',
      'wizard',
      'basic'
    ]);
  }

  openDecision(decisionId: number): void {
    this.router.navigate(
      ['/decisions', decisionId, 'wizard', 'preview'],
      {
        queryParams: {
          fromDecisionList: 'true'
        }
      }
    );
  }

  editDecision(
    decision: Decision
  ): void {
    this.router.navigate([
      '/decisions',
      decision.id,
      'wizard',
      'basic'
    ]);
  }

  confirmDelete(
    decision: Decision
  ): void {
    this.decisionToDelete = decision;

    this.deleteErrorMessage = '';
  }

  cancelDelete(): void {
    if (this.isDeleting) {
      return;
    }

    this.decisionToDelete = null;

    this.deleteErrorMessage = '';
  }

  deleteDecision(): void {
    if (!this.decisionToDelete) {
      return;
    }

    const decisionId =
      this.decisionToDelete.id;

    this.isDeleting = true;

    this.deleteErrorMessage = '';

    this.decisionService
      .deleteDecision(decisionId)
      .subscribe({

        next: () => {

          this.decisions =
            this.decisions.filter(
              decision =>
                decision.id !== decisionId
            );

          this.decisionToDelete = null;

          this.isDeleting = false;

          this.changeDetector.markForCheck();
        },

        error: (error) => {

          console.error(
            'Failed to delete decision',
            error
          );

          this.deleteErrorMessage =
            error?.error?.Error ??
            'Unable to delete the decision. Please try again.';

          this.isDeleting = false;

          this.changeDetector.markForCheck();
        }

      });
  }
}